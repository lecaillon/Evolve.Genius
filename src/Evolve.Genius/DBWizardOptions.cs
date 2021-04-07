using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Evolve.Genius
{
    public record DBWizardOptions : IValidatableObject
    {
        [Required]
        public string MetadataTableName { get; init; } = "_DBWizard";

        [MinLength(1)]
        public List<string> Locations { get; init; } = new();
        
        [Range(0, int.MaxValue)]
        public int CommandTimeout { get; init; } = 600;
        
        [Required]
        public GeniusSite MainSite { get; init; } = default!;
        
        [MinLength(1)]
        public List<GeniusSite> CompanySites { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(MainSite, new(MainSite, null), results);
            CompanySites.ForEach(site => Validator.TryValidateObject(site, new(site, null), results));

            return results;
        }
    }

    public record GeniusSite
    {
        [Required]
        public string Name { get; init; } = default!;

        [Required]
        public string SqlMigrationPrefix { get; init; } = default!;

        [Required]
        public string ConnectionString { get; init; } = default!;
    }
}
