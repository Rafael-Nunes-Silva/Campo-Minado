# Campo-Minado

Jogo baseado no Minesweeper feito em C#

<img width="400px" height="400px" src="https://user-images.githubusercontent.com/76886825/230950101-2ade9ee5-b0ed-41fa-b3de-7238ae8e4d3c.png">



# Como jogar
Os comandos durante o jogo são simples, o jogador deve escolher uma célula da tabela e digita-la no formato ```A15``` por exemplo.

Multiplas células podem ser inseridas de uma só vez, basta separa-las por um espaço ```B10 H2 Q20```.



## Comandos
* O jogador pode também marcar células com bandeiras (```&```) ao colocar o ```-f``` no final de seu comando.
* Ou destacar uma linha com o comando ```-h```, por exemplo: ```14 -h``` para destacar a linha 14.



# Multiplayer
O Campo-Minado conta com um modo multiplayer em que os jogadores podem criar salas para se reunirem e se desafiarem.

Para jogar o multiplayer do Campo-Minado, será necessário um servidor. Para aprender como rodar o servidor, dê uma olhada no repositório <a src="https://github.com/Rafael-Nunes-Silva/Campo-Minado-Server">Campo-Minado-Server</a>.
