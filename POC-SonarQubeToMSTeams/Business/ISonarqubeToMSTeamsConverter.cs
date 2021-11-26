using POC_SonarQubeToMSTeams.Models;

namespace POC_SonarQubeToMSTeams.Business
{
    public interface ISonarqubeToMSTeamsConverter
    {
        public MSTeamsComplexCard ToComplexCard(dynamic data, string culture);

    }
}
