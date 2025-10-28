namespace gh_kk.Interfaces;

public interface IGhIntegration
{
    string GetToken(bool verbose);
    string GetActiveUser(bool verbose);
    string GetHostname(bool verbose);
}
