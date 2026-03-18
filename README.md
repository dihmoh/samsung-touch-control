# Samsung Touch Control

> ⚡ Este projeto foi vibe coded com auxílio de IA (Claude - Anthropic). Code review e correções por Jules (Google AI). Ideia e direção por DihMoh.

Utilitário leve para controlar o touchscreen de notebooks Samsung diretamente pela bandeja do sistema.

## Por que criei isso?

Meu Samsung Galaxy Book 3 360 começou a apresentar **ghost touch** — a tela registrava toques fantasmas aleatórios no canto da tela sem eu estar tocando nela, clicando milhares de vezes sozinha.

A solução era desativar o touchscreen pelo Gerenciador de Dispositivos, mas fazer isso toda vez era frustrante pois era necessário reiniciar o notebook toda vez, fora o problema que era tentar mexer no notebook enquanto ele clicava várias vezes sozinho. Então criei esse utilitário para alternar o touch com um clique ou atalho de teclado!

Se você tem um Samsung com touchscreen e sofre com:
- 👻 Ghost touch / ghost clicks
- ✋ Toques acidentais enquanto digita
- 🖱️ Tela interferindo no uso do touchpad

Esse app é para você!

## Funcionalidades
- Ícone na bandeja do sistema com estado ON/OFF
- Ativar, desativar e alternar o touchscreen
- Tecla de atalho global (Ctrl+Alt+T)
- Popup de notificação ao mudar estado
- Iniciar automaticamente com o Windows
- Painel com botão ON/OFF
- Interface em Português e Inglês automático
- Detecção automática do touchscreen Samsung

## Dispositivos testados
| Modelo | Status |
|--------|--------|
| Samsung Galaxy Book 3 360 | ✅ Testado pelo autor |
| Samsung Galaxy Book 6 Ultra (ARC iGPU) | ✅ Testado pela comunidade |

## Requisitos
- Windows 10/11
- Notebook Samsung com touchscreen
- Sem necessidade de instalar .NET separadamente

## Como usar
1. Baixe e execute o instalador `SamsungTouchControl_Setup.exe`
2. Aceite a elevação de administrador
3. O app inicia automaticamente na bandeja do sistema
4. Clique com botão direito no ícone para o menu
5. Use **Ctrl+Alt+T** para alternar o touch

## Compatibilidade
Testado no Samsung Galaxy Book 3 360 e Galaxy Book 6 Ultra. Deve funcionar em qualquer notebook Samsung com touchscreen HID compatível.

## Contribuindo
Pull requests são bem-vindos! Para mudanças maiores, abra uma issue primeiro.

## Licença
MIT — faça o que quiser! 😄
