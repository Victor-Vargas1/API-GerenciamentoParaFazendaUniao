// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function formatarCPF(input) {
    let value = input.value.replace(/\D/g, ""); // Remove caracteres não numéricos
    value = value.replace(/(\d{3})(\d)/, "$1.$2"); // Adiciona o primeiro ponto
    value = value.replace(/(\d{3})(\d)/, "$1.$2"); // Adiciona o segundo ponto
    value = value.replace(/(\d{3})(\d{2})$/, "$1-$2"); // Adiciona o traço
    input.value = value;
}

function formatarTelefone(input) {
    let value = input.value.replace(/\D/g, ""); // Remove caracteres não numéricos
    value = value.replace(/(\d{2})(\d)/, "($1) $2"); // Adiciona os parênteses para DDD
    value = value.replace(/(\d{5})(\d)/, "$1-$2"); // Adiciona o hífen após os primeiros 5 dígitos
    input.value = value;
}

function formatarCNPJ(input) {
    let value = input.value.replace(/\D/g, ""); // Remove caracteres não numéricos
    value = value.replace(/(\d{2})(\d)/, "$1.$2"); // Adiciona o primeiro ponto
    value = value.replace(/(\d{3})(\d)/, "$1.$2"); // Adiciona o segundo ponto
    value = value.replace(/(\d{3})(\d{4})(\d{2})$/, "$1/$2-$3"); // Adiciona a barra e o traço
    input.value = value;
}

function formatarSalario(input) {
    let value = input.value.replace(/\D/g, ""); // Remove todos os caracteres não numéricos
    value = (parseFloat(value) / 100).toFixed(2); // Converte para decimal e adiciona duas casas decimais
    value = value.replace(/\B(?=(\d{3})+(?!\d))/g, "."); // Adiciona o separador de milhares
    input.value = "R$ " + value.replace(".", ","); // Converte o ponto decimal para vírgula e adiciona "R$"
}

function removerFormatoSalario(input) {
    input.value = input.value.replace(/[^0-9.,]/g, "").replace(",", "."); // Remove "R$", pontos e vírgula para o backend
}

// Função para validar o formulário e exibir o modal de confirmação
function validateAndShowModal() {
    const form = document.querySelector('form');

    if (form.checkValidity()) { // Verifica a validade do formulário
        showConfirmModal(); // Exibe o modal de confirmação
    } else {
        form.reportValidity(); // Exibe as mensagens de erro de validação
    }
}

// Exibe o modal de confirmação
function showConfirmModal() {
    document.getElementById("confirmModal").style.display = "block";
}

// Função que é chamada quando o usuário confirma o cadastro
function confirmAction() {
    closeModal(); // Fecha o modal de confirmação
    const form = document.querySelector('form');
    form.submit(); // Submete o formulário

    // Simulação de sucesso no cadastro
    setTimeout(function () {
        showSuccessModal(); // Exibe o modal de sucesso após a submissão do formulário
    }, 5000); // Atraso de 500ms para garantir que o envio foi feito
}

// Função para exibir o modal de sucesso
function showSuccessModal() {
    document.getElementById("successModal").style.display = "block"; // Exibe o modal de sucesso
}

// Função para fechar o modal de confirmação
function closeModal() {
    document.getElementById("confirmModal").style.display = "none";
}

function confirmDelete() {
    // Exibe a confirmação antes de deletar
    document.getElementById("deleteForm").submit();
}

function showDeleteModal() {
    document.getElementById("confirmModal").style.display = "block";
}

function showEditConfirmation() {
    document.getElementById("confirmModal").style.display = "block";
}


// Função para confirmar a edição e submeter o formulário
function confirmEdit() {
    document.getElementById("editForm").submit(); // Submete o formulário de edição
}


// Eventos de formatação dos campos (CPF, telefone, CNPJ, salário)
document.addEventListener('DOMContentLoaded', function () {
    const cpfInput = document.getElementById('cpf');
    const telefoneInput = document.getElementById('telefone');
    const salarioInput = document.getElementById('salario');
    const cnpjInput = document.getElementById('cnpj');

    if (cpfInput) cpfInput.addEventListener('input', function () { formatarCPF(cpfInput); });
    if (telefoneInput) telefoneInput.addEventListener('input', function () { formatarTelefone(telefoneInput); });
    if (cnpjInput) cnpjInput.addEventListener('input', function () { formatarCNPJ(cnpjInput); });

    if (salarioInput) {
        salarioInput.addEventListener('input', function () { formatarSalario(salarioInput); });
        salarioInput.addEventListener('blur', function () { removerFormatoSalario(salarioInput); }); // Remove a máscara ao enviar o valor
    }
})
