# POC-SonarQubeToMSTeams
Esse projeto é resultado de um estudo sobre a integração do SonarQube com o MS Teams, e foi baseado em um [projeto](https://github.com/highbyte/SonarqubeMSTeamsBridge#configure-sonarqube) desenvolvido por Christer Cederborg AKA @Highbyte.

Não há até o presente momento um plugin que realize a integração do SonarQube com o MS Teams. Para realizar essa tarefa é necessário criar um serviço próprio.

## Durante as pesquisas realizadas, foram encontrados os seguintes desafios:

- O JSON que será enviado ao teams deverá estar no formato ["Adaptative Card"](https://docs.microsoft.com/pt-br/outlook/actionable-messages/adaptive-card)
- Configurar um [Incoming Webhook](https://docs.microsoft.com/en-us/microsoftteams/platform/webhooks-and-connectors/how-to/add-incoming-webhook) no canal do MS Teams;
- Configurar um [Webhook](https://docs.sonarqube.org/latest/project-administration/webhooks/) com a url da Azure Function no SonarQube;
- Realizar uma requisição POST para o endpoint do conector do Teams;

### A solução proposta é criar uma Azure Function que seja capaz de receber as requisições do SonarQube, converter o JSON para o formato adequado e realizar um POST para o conector configurado no MS Teams.

Após as instruções mencionadas no projeto desenvolvido por Christer Cederborg, foi possível elaborar uma prova de conceito que fosse capaz de realizar o propósito desse estudo. Segue abaixo imagem que demonstra a publicação no MS Teams feita pela Azure Function.

![POC-SonarQubeMSTeamsBridge](https://user-images.githubusercontent.com/44754775/143882720-57a8a182-aa9e-4d4d-838f-fcb23938562f.png)
Fonte: (O Autor)

Inicialmente, a ideia era que fosse possível mostrar o resultado das métricas que o SonarQube avalia, como por exemplo a porcentagem de cobertura de código. A imagem abaixo mostra como essa informação é apresentada no SonarQube.

![SonarQubeResultsExample](https://user-images.githubusercontent.com/44754775/143882900-6b6155c4-28bc-41fc-aacb-0d89aa3be00f.png)
Fonte: (SonarQube)

No entanto, o JSON que é fornecido pelo SonarQube não inclui essa informação. Podemos ver no trecho de código abaixo que ele apenas apresenta "OK", "ERROR" ou "NO_VALUE", informando o status do resultado das métricas.

```
{
    "serverUrl": "http://localhost:9000",
    "taskId": "AVh21JS2JepAEhwQ-b3u",
    "status": "SUCCESS",
    "analysedAt": "2016-11-18T10:46:28+0100",
    "revision": "c739069ec7105e01303e8b3065a81141aad9f129",
    "project": {
        "key": "myproject",
        "name": "My Project",
        "url": "https://mycompany.com/sonarqube/dashboard?id=myproject"
    },
    "properties": {
    },
    "qualityGate": {
        "conditions": [
            {
                "errorThreshold": "1",
                "metric": "new_security_rating",
                "onLeakPeriod": true,
                "operator": "GREATER_THAN",
                "status": "OK",
                "value": "1"
            },
            {
                "errorThreshold": "1",
                "metric": "new_reliability_rating",
                "onLeakPeriod": true,
                "operator": "GREATER_THAN",
                "status": "ERROR",
                "value": "1"
            },
            {
                "errorThreshold": "1",
                "metric": "new_maintainability_rating",
                "onLeakPeriod": true,
                "operator": "GREATER_THAN",
                "status": "OK",
                "value": "1"
            },
            {
                "errorThreshold": "80",
                "metric": "new_coverage",
                "onLeakPeriod": true,
                "operator": "LESS_THAN",
                "status": "NO_VALUE"
            }
        ],
        "name": "SonarQube way",
        "status": "OK"
    }
}
```

Dessa forma, optamos por disponibilizar um botão que quando clicado, o usuário é redirecionado para o link do projeto no SonarQube que contém todas as informações pertinentes às métricas disponíveis para consulta.

---

### Passo a passo para a conclusão da integração:

1. Clonar o projeto;
2. [Definir o formato visual](https://amdesigner.azurewebsites.net/) da mensagem bem como as infos que serão apresentadas no canal do MS Teams e realizar as alterações no código;
3. Realizar o Deploy do projeto;
4. Criar um [Incoming Webhook](https://docs.microsoft.com/en-us/microsoftteams/platform/webhooks-and-connectors/how-to/add-incoming-webhook)  no MS Teams e inserir o respectivo link fornecido no projeto;
5. [Configurar um Webhook](https://docs.sonarqube.org/latest/project-administration/webhooks/) no SonarQube utilizando o endpoint do recurso de Azure Function criado.

---
## Referências:
- CEDERBORG, C. Projeto SonarQube MS Teams Bridge. Github, 2020. Disponível em: <https://github.com/highbyte/SonarqubeMSTeamsBridge>. Acesso em: 19 de novembro de 2021.
- JOHNSTON, J. Olprod. Criação de cartões de mensagens acionáveis do Outlook com o formato Cartão Adaptável. Microsoft, 2021. Disponível em: <https://docs.microsoft.com/pt-br/outlook/actionable-messages/adaptive-card>. Acesso em: 19 de novembro de 2021.
- laujan et. al. Create Incoming Webhook. Microsoft, 2021. Disponível em: <https://docs.microsoft.com/en-us/microsoftteams/platform/webhooks-and-connectors/how-to/add-incoming-webhook>. Acesso em: 19 de novembro de 2021.
- SonarSource. Docs 9.2. Webhooks. Disponível em: <https://docs.sonarqube.org/latest/project-administration/webhooks/>. Acesso em 19 de novembro de 2021.
