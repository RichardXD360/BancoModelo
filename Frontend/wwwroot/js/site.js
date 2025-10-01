// --- Helpers ---
const $ = id => document.getElementById(id);
const apiBase = 'https://localhost:7067';

const usuarioId = localStorage.getItem("usuarioId");

console.log(usuarioId);

loadExtrato();
function formatCurrency(v) {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(v);
}
function formatDate(iso) {
    if (!iso) return '-';
    const d = new Date(iso);
    return d.toLocaleString('pt-BR');
}

// --- Panel navigation ---
document.querySelectorAll('.nav-btn').forEach(btn => btn.addEventListener('click', e => {
    const id = e.target.id;
    if (id.includes('extrato')) showPanel('panel-extrato');
    if (id.includes('transferencia')) showPanel('panel-transferencia');
    if (id.includes('dados')) showPanel('panel-dados');
}));

function showPanel(panelId) {
    document.querySelectorAll('.panel').forEach(p => p.classList.add('hidden'));
    $(panelId).classList.remove('hidden');
}

// Dark mode toggle
const darkToggle = $('toggle-dark');
darkToggle.addEventListener('click', () => {
    document.documentElement.classList.toggle('dark');
    darkToggle.textContent = document.documentElement.classList.contains('dark') ? 'Modo Claro' : 'Modo Escuro';
});

// --- Extrato ---
let extratoData = [];
async function loadExtrato() {
    const id = usuarioId;
    try {
        const res = await axios.get(`${apiBase}/Usuario/DetalhesUsuario/${id}`);
        const payload = res.data;
        if (!payload || !payload.detalhesUsuario) {
            $('extrato-summary').textContent = 'Resposta inválida da API.';
            return;
        }
        const det = payload.detalhesUsuario;
        extratoData = (det.transacao || []).map(t => ({ ...t, dataTransacao: new Date(t.dataTransacao) }));
        // sort desc by date
        extratoData.sort((a, b) => b.dataTransacao - a.dataTransacao);
        $('extrato-summary').textContent = `Nome: ${det.nome} — CPF: ${det.cpf} — Saldo: ${formatCurrency(det.saldo)}`;
        renderExtratoTable();
        // update dados panel as well
        updateDadosPanel(det);
    } catch (err) {
        console.error(err);
        $('extrato-summary').textContent = 'Erro ao carregar extrato. Verifique API.';
    }
}

function renderExtratoTable() {
    const tbody = $('extrato-tbody');
    const filterText = $('filter-text').value.toLowerCase();
    const start = $('filter-start').value ? new Date($('filter-start').value) : null;
    const end = $('filter-end').value ? new Date($('filter-end').value) : null;
    tbody.innerHTML = '';
    const rows = extratoData.filter(t => {
        if (filterText) {
            const inText = (t.transacaoId || '') || (t.descricao || '').toLowerCase().includes(filterText) || (t.usuarioId || '').toString().includes(filterText) || (t.usuarioRecebedorId || '').toString().includes(filterText) || (t.valor || '').toString().includes(filterText);
            if (!inText) return false;
        }
        if (start && t.dataTransacao < start) return false;
        if (end) {
            const endDay = new Date(end); endDay.setHours(23, 59, 59, 999);
            if (t.dataTransacao > endDay) return false;
        }
        return true;
    });

    if (rows.length === 0) {
        tbody.innerHTML = '<tr><td class="px-3 py-2" colspan="6">Nenhuma transação encontrada.</td></tr>';
        return;
    }

    for (const t of rows) {
        const tr = document.createElement('tr');
        tr.className = 'border-t';
        tr.innerHTML = `
              <td class="px-3 py-2"> <button id="btnGerarComprovante" onClick=GerarComprovante(${t.transacaoId}) type="submit"
                        class="w-full inline-flex justify-center items-center gap-2 px-2 py-3 rounded-lg font-medium text-white bg-purple-600 hover:bg-purple-700 focus-ring">
                    Baixar Comprovante
                </button>  </td>
              <td class="px-3 py-2">${formatDate(t.dataTransacao)}</td>
              <td class="px-3 py-2">${t.descricao || '-'}</td>
              <td class="px-3 py-2">${formatCurrency(t.valor)}</td>
              <td class="px-3 py-2">${mapTipo(t.tipoTransacao)}</td>
              <td class="px-3 py-2">${t.usuarioId ?? '-'}</td>
              <td class="px-3 py-2">${t.usuarioRecebedorId ?? '-'}</td>
            `;
        tbody.appendChild(tr);
    }
}

async function GerarComprovante(transacaoId) {
    try {
        const res = await axios.get(`${apiBase}/Transacao/${transacaoId}/GerarComprovante`);
        const basePdf = res.data;

        const link = document.createElement("a");
        link.href = "data:application/pdf;base64," + basePdf;
        link.download = "bf_comprovante.pdf";
        link.click();
    } catch(err) {
        
    }
}

function mapTipo(v) {
    switch (v) {
        case 1: return 'DEBITO';
        case 2: return 'CREDITO';
        case 3: return 'PIX';
        case 4: return 'TRANSFERENCIA';
        default: return v;
    }
}

$('btn-load-extrato').addEventListener('click', loadExtrato);
$('filter-text').addEventListener('input', renderExtratoTable);
$('filter-start').addEventListener('change', renderExtratoTable);
$('filter-end').addEventListener('change', renderExtratoTable);

// --- Transferência ---
$('btn-send-tx').addEventListener('click', async () => {
    const valor = parseFloat($('tx-valor').value || '0');
    const tipo = parseInt($('tx-tipo').value);
    const descricao = $('tx-descricao').value || null;
    const cnpj = $('tx-receiver-cnpj').value || null;

    const payload = {
        usuarioId: usuarioId,
        valor: valor,
        tipoTransacao: tipo,
        dataTransacao: new Date().toISOString(),
        descricao: descricao,
        usuarioRecebedorCnpj: cnpj
    };

    $('tx-result').textContent = 'Enviando...';
    try {
        const res = await axios.post(`${apiBase}/Transacao/EfetuarTransacao`, payload);
        $('tx-result').innerHTML = `<span class="text-green-600 dark:text-green-400">Sucesso: ${JSON.stringify(res.data.mensagem)}</span>`;
        await loadExtrato();
    } catch (err) {
        console.error(err);
        const msg = err?.response?.data ? JSON.stringify(err.response.data.mensagem) : (err.message || 'Erro');
        $('tx-result').innerHTML = `<span class="text-red-600 dark:text-red-400">Falha: ${msg}</span>`;
    }
});

// --- Dados do usuário ---
function updateDadosPanel(detalhesUsuario) {
    if (!detalhesUsuario) return;
    $('user-saldo').textContent = formatCurrency(detalhesUsuario.saldo || 0);
    const dadosHtml = `
            <div><strong>Nome:</strong> ${detalhesUsuario.nome}</div>
            <div><strong>CPF:</strong> ${detalhesUsuario.cpf}</div>
            <div><strong>AgênciaId:</strong> ${detalhesUsuario.agenciaId}</div>
          `;
    $('user-dados').innerHTML = dadosHtml;

    const last = (detalhesUsuario.transacao || []).slice().sort((a, b) => new Date(b.dataTransacao) - new Date(a.dataTransacao))[0];
    if (last) {
        $('user-ultima').innerHTML = `${formatDate(last.dataTransacao)} — ${last.descricao || '-'} — ${formatCurrency(last.valor)}`;
    } else {
        $('user-ultima').textContent = 'Nenhuma transação registrada.';
    }
}

// Initialize
showPanel('panel-extrato');
// optional: preload extrato for default id
// loadExtrato();