using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using System.Net;

namespace license
{

    public class KeygenService
    {
        private readonly RestClient client;
        private readonly ILogger<KeygenService> log;

        public KeygenService(IOptions<KeygenOptions> options, ILogger<KeygenService> log)
        {
            client = new RestClient(options?.Value?.AccountId ?? throw new ArgumentNullException("AccountId"));
            this.log = log;
        }

        async public Task<DocumentWithMeta<License, Validation>> ValidateLicense(string licenseKey, string deviceFingerprint)
        {
            var request = new RestRequest("licenses/actions/validate-key", Method.Post);

            request.AddHeader("Content-Type", "application/vnd.api+json");
            request.AddHeader("Accept", "application/vnd.api+json");
            request.AddJsonBody(new
            {
                meta = new
                {
                    key = licenseKey,
                    scope = new
                    {
                        fingerprint = deviceFingerprint,
                    }
                }
            });

            var response = await client.ExecuteAsync<DocumentWithMeta<License, Validation>>(request);
            return response.Data;
        }

        async public Task<bool> IncrementUsage(string licenseId, string activationToken, int value)
        {
            var request = new RestRequest($"licenses/{licenseId}/actions/increment-usage", Method.Post);

            request.AddHeader("Authorization", $"Bearer {activationToken}");
            request.AddHeader("Content-Type", "application/vnd.api+json");
            request.AddHeader("Accept", "application/vnd.api+json");
            request.AddJsonBody(new
            {
                meta = new
                {
                    increment = value
                }
            });

            var response = await client.ExecuteAsync<DocumentWithMeta<License, Validation>>(request);
            return response.IsSuccessful;
        }

        async public Task<IList<Entitlement>> Entitlements(string licenseId, string activationToken)
        {
            var request = new RestRequest($"licenses/{licenseId}/entitlements", Method.Get);

            request.AddHeader("Authorization", $"Bearer {activationToken}");
            request.AddHeader("Content-Type", "application/vnd.api+json");
            request.AddHeader("Accept", "application/vnd.api+json");
            var response = await client.ExecuteAsync<Document<IList<Entitlement>>>(request);
            return response.IsSuccessful ? response.Data?.Data : null;
        }

        async public Task<Document<Machine>> ActivateDevice(string licenseId, string deviceFingerprint, string activationToken, Dictionary<string, object> metadata = null)
        {
            var request = new RestRequest("machines", Method.Post);
            string? ipAddress = null;
            string? hostname = null;
            if (metadata == null)
                metadata = new Dictionary<string, object>();

            try
            {
                var rsp = await client.GetAsync<IpApiResponse>(new RestRequest("http://ip-api.com/json/")).ConfigureAwait(false);
                if (rsp?.status == "success")
                {
                    metadata["country"] = rsp.country;
                    metadata["region"] = rsp.regionName;
                    metadata["city"] = rsp.city;
                    metadata["zipCode"] = rsp.zip;
                    metadata["org"] = rsp.org;
                    metadata["lat"] = rsp.lat;
                    metadata["long"] = rsp.lon;
                    ipAddress = rsp.query;
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Unable to use ip-api.com for location.");
                try
                {
                    var ip = await client.GetAsync(new RestRequest("https://whatsmyip.on-it.xyz/")).ConfigureAwait(false);
                    ipAddress = ip.IsSuccessful ? ip.Content : null;
                }
                catch (Exception ex2)
                {
                    log.LogError(ex2, "Unable to get ip from https://whatsmyip.on-it.xyz/");
                }
            }

            if (!string.IsNullOrEmpty(ipAddress))
            {
                try
                {
                    var hostnames = await Dns.GetHostAddressesAsync(ipAddress);
                    hostname = hostnames.FirstOrDefault()?.ToString();
                }
                catch (Exception ex)
                {
                    log.LogError(ex, "Unable to resolve hostname from ip {ip}", ipAddress);
                }
            }

            request.AddHeader("Authorization", $"Bearer {activationToken}");
            request.AddHeader("Content-Type", "application/vnd.api+json");
            request.AddHeader("Accept", "application/vnd.api+json");
            request.AddJsonBody(new
            {
                data = new
                {
                    type = "machine",
                    attributes = new
                    {
                        fingerprint = deviceFingerprint,
                        platform = Environment.OSVersion.ToString(),
                        name = Environment.MachineName,
                        ip = ipAddress,
                        hostname = hostname,
                        cores = Environment.ProcessorCount,
                        metadata = metadata
                    },
                    relationships = new
                    {
                        license = new
                        {
                            data = new
                            {
                                type = "license",
                                id = licenseId,
                            }
                        }
                    }
                }
            });

            var response = await client.ExecuteAsync<Document<Machine>>(request).ConfigureAwait(false);
            return response.Data;
        }
    }
}
