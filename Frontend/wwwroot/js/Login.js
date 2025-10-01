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

document.getElementById('loginForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    ['loginError', 'cpfError'].forEach(hideError);
    const messageBox = document.getElementById('loginMessage');
    messageBox.textContent = '';

    const login = document.getElementById('loginInput').value.trim();
    const cpf = document.getElementById('cpfInput').value.trim();

    let ok = true;
    if (!login) { showError('loginError', 'Preencha o login.'); ok = false; }
    if (!cpfRegex.test(cpf)) { showError('cpfError', 'CPF inválido. Use o formato 000.000.000-00.'); ok = false; }
    if (!ok) return;

    const body = { Login: login, Cpf: cpf };

    try {
        const btn = document.getElementById('btnVerificar');
        btn.disabled = true;
        btn.classList.add('opacity-70', 'cursor-not-allowed');

        const res = await axios.post('https://localhost:7067/Usuario/VerificarUsuario', body, {
            headers: { 'Content-Type': 'application/json' }
        });

        const usuarioId = res.data.usuarioId;

        localStorage.setItem("usuarioId", usuarioId);

        messageBox.className = 'text-sm mt-2 text-green-600';
        messageBox.textContent = 'Usuário verificado com sucesso.';
        if (usuarioId != undefined) {
            window.location.href = "https://localhost:7028/Home/index";
        }
    } catch (err) {
        console.error(err);
        messageBox.className = 'text-sm mt-2 text-red-600';
        messageBox.textContent = `Erro: ${err.data.Mensagem}`;
    } finally {
        const btn = document.getElementById('btnVerificar');
        btn.disabled = false;
        btn.classList.remove('opacity-70', 'cursor-not-allowed');
    }
});