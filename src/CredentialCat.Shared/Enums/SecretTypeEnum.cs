namespace CredentialCat.Shared.Enums
{
    /// <summary>
    /// Enumerate the type of the secret
    /// </summary>
    public enum SecretTypeEnum
    {
        PlainText,
        Hashed,
        Mixed = PlainText | Hashed,
        Unknown
    }
}