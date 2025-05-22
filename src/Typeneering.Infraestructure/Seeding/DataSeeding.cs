using Typeneering.Domain.Preference.Entities;

namespace Typeneering.Infraestructure.Seeding;

public class DataSeeding
{
    public IReadOnlyCollection<PreferenceEntity> PreferencesData => [
        new PreferenceEntity {
            Name = "--background-color",
            CreatedAt = DateTimeOffset.UtcNow
        },
        new PreferenceEntity {
            Name = "--text-color",
            CreatedAt = DateTimeOffset.UtcNow
        },
        new PreferenceEntity {
            Name = "--input-color",
            CreatedAt = DateTimeOffset.UtcNow
        },
        new PreferenceEntity {
            Name = "--typing-text-color",
            CreatedAt = DateTimeOffset.UtcNow
        },
        new PreferenceEntity {
            Name = "--whitespace-border-color",
            CreatedAt = DateTimeOffset.UtcNow
        },
        new PreferenceEntity {
            Name = "--incorrect-background-color",
            CreatedAt = DateTimeOffset.UtcNow
        },
        new PreferenceEntity {
            Name = "--typing-overlay-color",
            CreatedAt = DateTimeOffset.UtcNow
        },
        new PreferenceEntity {
            Name = "--correct-text-color",
            CreatedAt = DateTimeOffset.UtcNow
        },
        new PreferenceEntity {
            Name = "--correct-text-background-color",
            CreatedAt = DateTimeOffset.UtcNow
        },
        new PreferenceEntity {
            Name = "--incorrect-text-color",
            CreatedAt = DateTimeOffset.UtcNow
        },
        new PreferenceEntity {
            Name = "--incorrect-text-background-color",
            CreatedAt = DateTimeOffset.UtcNow
        },
        new PreferenceEntity {
            Name = "--cursor-color",
            CreatedAt = DateTimeOffset.UtcNow
        },
        new PreferenceEntity {
            Name = "--typing-font-family",
            CreatedAt = DateTimeOffset.UtcNow
        },
        new PreferenceEntity {
            Name = "--typing-font-size",
            CreatedAt = DateTimeOffset.UtcNow
        },
        new PreferenceEntity {
            Name = "--typing-font-weight",
            CreatedAt = DateTimeOffset.UtcNow
        },
    ];
}
