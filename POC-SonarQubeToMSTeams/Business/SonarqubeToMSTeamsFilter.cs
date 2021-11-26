﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC_SonarQubeToMSTeams.Business
{
    public class SonarqubeToMSTeamsFilter : ISonarqubeToMSTeamsFilter
    {
        public bool ShouldProcess(dynamic data, string qualityGateStatusExcludes)
        {
            bool allowedQualityGateStatus = AllowedQualityGateStatus(data, qualityGateStatusExcludes);
            return allowedQualityGateStatus;
        }

        public bool AllowedQualityGateStatus(dynamic data, string qualityGateStatusExcludes)
        {
            // If not specifying any Quality Gate Status exclude filter, allow any status.
            if (string.IsNullOrEmpty(qualityGateStatusExcludes))
                return true;

            string[] qualityGateStatusExcludeList = qualityGateStatusExcludes.Split(",");
            return !qualityGateStatusExcludeList.Contains((string)data.qualityGate.status, StringComparer.CurrentCultureIgnoreCase);
        }

    }
}
