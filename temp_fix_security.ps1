# Este script remove o certificado de autenticação do histórico do Git para corrigir uma falha de segurança.
# O arquivo não será apagado do seu disco, apenas do controle de versão.

Write-Host "Passo 1: Removendo 'src/certs/academyio-auth.pfx' do índice do Git..."
git rm --cached src/certs/academyio-auth.pfx

Write-Host "Passo 2: Criando o commit da alteração de segurança..."
git commit -m "security: Remove certificate from version control"

Write-Host "Concluído! O certificado não está mais no controle de versão."
Write-Host "Este script (temp_fix_security.ps1) pode ser apagado."
