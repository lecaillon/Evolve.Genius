using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Evolve.Migration;
using Microsoft.Extensions.Logging;

namespace Evolve.Genius
{
    public record DBWizard(DBWizardOptions Options, ILogger Logger)
    {
        public void Migrate()
        {
            foreach (GeniusVersion migration in GetMigrations().Keys)
            {
                Logger.LogInformation($"Upgrading Genius to version {migration}...");

                MigrateSite(migration, Options.MainSite);

                foreach (GeniusSite site in Options.CompanySites)
                {
                    MigrateSite(migration, site);
                }

                Logger.LogInformation($"Succesfully upgraded Genius to version {migration} ! {Environment.NewLine}");
            }
        }

        private void MigrateSite(GeniusVersion migration, GeniusSite site)
        {
            Logger.LogInformation($"Upgrading site {site.Name}...");
            var evolve = new Evolve(new SqlConnection(site.ConnectionString), msg => Logger.LogInformation(msg))
            {
                Locations = Options.Locations,
                CommandTimeout = Options.CommandTimeout,
                EnableClusterMode = false,
                SqlMigrationPrefix = site.SqlMigrationPrefix,
                TargetVersion = new MigrationVersion($"{migration}.9999"),
            };

            evolve.Migrate();
        }

        private Dictionary<GeniusVersion, List<GeniusFullVersion>> GetMigrations()
        {
            return new FileMigrationLoader(Options.Locations)
                .GetMigrations(Options.MainSite.SqlMigrationPrefix, "__", ".sql")
                .Select(x => new GeniusFullVersion(
                    Version: new(x.Version!.VersionParts[0], x.Version!.VersionParts[1], x.Version!.VersionParts[2]),
                    Patch: x.Version!.VersionParts[3]))
                .GroupBy(x => x.Version)
                .ToDictionary(x => x.Key, x => x.ToList());
        }
    }
}
