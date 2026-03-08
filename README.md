# Myst of WoodEdge

Um jogo de ação e sobrevivência 2D desenvolvido em Unity, onde você luta contra hordas de monstros em uma floresta mística.

## 📋 Sobre o Projeto

Myst of WoodEdge é um jogo de ação RPG onde o jogador explora um mapa infinito toroidal, enfrentando diversos tipos de inimigos e chefes. O jogo apresenta um sistema de progressão com coleta de moedas, experiência, e melhorias que podem ser adquiridas na loja.

## 🎮 Características

- **Sistema de Combate**: Diversos ataques e projéteis (bolas de fogo, laser, folhas orbitais, machado)
- **Inimigos Variados**: 5 tipos diferentes de monstros com comportamentos únicos
- **Boss Battles**: Enfrentamentos épicos contra chefes poderosos
- **Sistema de Progressão**: Colete XP e moedas para melhorar seu personagem
- **Loja**: Sistema de compras para adquirir upgrades e melhorias
- **Mapa Infinito**: Geração procedural com sistema toroidal
- **Sistema de Stats**: Velocidade, dano de ataque e taxa de ataque customizáveis

## 🛠️ Tecnologias Utilizadas

- **Unity** (versão 2D)
- **C#** para scripting
- **Unity Input System** para controles
- **TextMesh Pro** para UI
- **Universal Render Pipeline (URP)**

## 📁 Estrutura do Projeto

```
Assets/
├── Animation/      # Animações de personagens e efeitos
├── Audios/         # Arquivos de som e música
├── Fonts/          # Fontes para UI
├── Prefab/         # Prefabs reutilizáveis
├── Scenes/         # Cenas do jogo
│   ├── MainMenu.unity
│   ├── SampleScene.unity (Jogo principal)
│   ├── Lojinha.unity
│   ├── GameOver.unity
│   └── GameWin.unity
├── Scripts/        # Todos os scripts C#
├── Sprites/        # Sprites e assets visuais
└── Tile/          # Tilesets para o mapa
```

## 🎯 Como Jogar

1. Abra o projeto no Unity Editor
2. Navegue até `Assets/Scenes/MainMenu.unity`
3. Clique em Play para iniciar o jogo
4. Use os controles configurados no Input System para mover e atacar
5. Sobreviva contra as ondas de inimigos
6. Colete moedas e XP para melhorar seu personagem
7. Visite a loja para comprar upgrades

## 🎨 Assets Principais

- **Personagens**: Pink Monster, Owlet Monster, Dude Monster e mais
- **Projéteis**: Bola de fogo, laser, folhas, raios
- **UI**: Frames, botões, e ícones customizados
- **Tiles**: Sistema de mapa com tileset de floresta

## 🔧 Sistema de Scripts

- `PlayerController.cs`: Controla movimento e ações do jogador
- `GameManager.cs`: Gerencia o estado global do jogo
- `EnemyController.cs`: Comportamento dos inimigos
- `BossController.cs`: Lógica dos chefes
- `ShopUI.cs`: Interface da loja
- `ToroidalLargeMapGenerator.cs`: Geração de mapa infinito
- `PlayerStats.cs`: Sistema de estatísticas do jogador

## 📝 Requisitos

- Unity 2021.3 ou superior
- Windows, macOS, ou Linux
- Input System Package (já incluído)