const cpfRegex = /^\d{3}\.\d{3}\.\d{3}-\d{2}$/;

function showError(elId, message) {
    const el = document.getElementById(elId);
    el.textContent = message;
    el.classList.remove('hidden');
}
function hideError(elId) {
    const el = document.getElementById(elId);
    el.textContent = '';
    el.classList.add('hidden');
}

document.getElementById('registerForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    ['nomeCompletoError', 'usernameError', 'cpfError', 'agenciaError'].forEach(hideError);
    const messageBox = document.getElementById('formMessage');
    messageBox.textContent = '';

    const nome = document.getElementById('nomeCompleto').value.trim();
    const login = document.getElementById('username').value.trim();
    const cpf = document.getElementById('cpf').value.trim();
    const agenciaVal = document.getElementById('agencia').value;

    let ok = true;
    if (nome.length < 8 || nome.length > 48) { showError('nomeCompletoError', 'Nome deve ter entre 8 e 48 caracteres.'); ok = false; }
    if (login.length < 8 || login.length > 18) { showError('usernameError', 'Usuário deve ter entre 8 e 18 caracteres.'); ok = false; }
    if (!cpfRegex.test(cpf)) { showError('cpfError', 'CPF inválido.'); ok = false; }
    if (!agenciaVal) { showError('agenciaError', 'Selecione uma agência.'); ok = false; }

    if (!ok) return;

    const body = {
        Nome: nome,
        Login: login,
        Cpf: cpf,
        AgenciaId: parseInt(agenciaVal, 10)
    };

    try {
        const btn = document.getElementById('btnCadastrar');
        btn.disabled = true;
        btn.classList.add('opacity-70', 'cursor-not-allowed');

        await axios.post('https://localhost:7067/CriarUsuario', body, {
            headers: { 'Content-Type': 'application/json' }
        });

        messageBox.className = 'text-sm mt-2 text-green-600';
        messageBox.textContent = 'Usuário cadastrado com sucesso.';

    } catch (err) {
        console.error(err);
        messageBox.className = 'text-sm mt-2 text-red-600';
        messageBox.textContent = `Erro ao cadastrar usuário: ${err.data.Mensagem}`;
    } finally {
        const btn = document.getElementById('btnCadastrar');
        btn.disabled = false;
        btn.classList.remove('opacity-70', 'cursor-not-allowed');
    }
});