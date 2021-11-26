using System;
using System.Collections.Generic;
using System.Text;

namespace POC_SonarQubeToMSTeams.Business
{
    public interface ISonarqubeToMSTeamsFilter
    {
        public bool ShouldProcess(dynamic data, string qualityGateStatusExcludes);

    }
}
