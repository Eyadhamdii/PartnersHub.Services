namespace PartnersHub.ConfigurationHub.Domain.Enums;

/// <summary>
/// Type of terms and conditions
/// </summary>
public enum TermsType : byte {
    Global = 0,
    Synergy = 1,
    Communities = 2,
    InfraBase = 3,
    PIFComp = 4
}

/// <summary>
/// Status of terms and conditions version
/// </summary>
public enum TermsStatus : byte {
    Draft = 0,
    Active = 1,
    Superseded = 2,
    Archived = 3
}