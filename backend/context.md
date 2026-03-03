## O Projeto: "Auction-House-Engine"

### 1. Funcionalidades Principais

* **Lances em Tempo Real:** Usuários dão lances e todos os outros conectados veem o valor subir instantaneamente sem atualizar a página.
* **Gateway de Saldo (Wallet):** O usuário precisa ter um saldo "bloqueado" ou pré-autorizado para dar um lance (garantia de pagamento).
* **Monitoramento de Encerramento:** Um serviço de background que verifica o cronômetro de cada leilão e, ao chegar em zero, processa a transação financeira final.

### 2. Stack Técnica e Arquitetura

Para esse projeto, a **Clean Architecture** é essencial para manter o código testável e organizado.

* **API Principal (ASP.NET Core):** Gerencia o CRUD de produtos e leilões.
* **SignalR:** O coração do projeto. Notifica os clientes sobre novos lances e o fechamento do cronômetro.
* **Worker Service (Background Processing):** Um serviço que roda 24/7 verificando no banco de dados quais leilões expiraram para processar o vencedor.
* **Entity Framework Core + SQL Server:** Para garantir a integridade das transações financeiras (ACID).
* **Redis:** Armazena o "lance atual" em memória para que a leitura seja extremamente rápida, já que milhares de pessoas podem consultar o preço ao mesmo tempo.

---

### 3. O Fluxo do Sistema (Exemplo)

| Etapa | Componente .NET | O que acontece? |
| --- | --- | --- |
| **Lance** | `SignalR Hub` | O usuário envia um lance. O Hub valida se o valor é maior que o atual e se o usuário tem saldo. |
| **Broadcast** | `SignalR + Redis` | O novo valor é salvo no Redis e disparado para todos os dispositivos conectados. |
| **Fechamento** | `Worker Service` | O serviço detecta que o tempo acabou, chama o `PaymentService` para debitar do vencedor e gerar a "Nota Fiscal". |
| **Notificação** | `MassTransit / RabbitMQ` | Dispara um e-mail/notificação confirmando a arrematação do item. |

---

### Por que isso é um excelente projeto de portfólio?

1. **Concorrência:** Você terá que lidar com o problema de dois usuários darem o mesmo lance no exato milissegundo (usando *Pessimistic Locking* ou *Redis Distributed Locks*).
2. **Escalabilidade:** Mostra que você sabe quando usar um Banco Relacional (SQL) para dinheiro e NoSQL (Redis) para velocidade.
3. **Complexidade Real:** Resolve problemas reais de sistemas que não podem falhar e precisam ser rápidos.