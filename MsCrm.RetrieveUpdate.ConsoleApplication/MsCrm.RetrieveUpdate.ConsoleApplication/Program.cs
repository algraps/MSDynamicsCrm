using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace MsCrm.RetrieveUpdate.ConsoleApplication
{
    class Program
    {
        private static OrganizationServiceProxy _serviceProxy;

        static void Main(string[] args)
        {
            try
            {
                IServiceConfiguration<IOrganizationService> orgConfigInfo =
                ServiceConfigurationFactory.CreateConfiguration<IOrganizationService>(new Uri("http://server/Organization/XRMServices/2011/Organization.svc"));
                
                System.Net.NetworkCredential userDefined = new System.Net.NetworkCredential("Username", "Password", "DomainServer");

                var creds = new ClientCredentials();
                creds.Windows.ClientCredential = userDefined;

                using (_serviceProxy = new OrganizationServiceProxy(orgConfigInfo, creds))
                {
                    RetrieveAllEntitiesRequest request = new RetrieveAllEntitiesRequest()
                    {
                        EntityFilters = EntityFilters.Entity,
                        RetrieveAsIfPublished = true
                    };

                    // Retrieve the MetaData.
                    RetrieveAllEntitiesResponse response = (RetrieveAllEntitiesResponse)_serviceProxy.Execute(request);
                    Console.WriteLine(string.Format("Entities: {0}", response.EntityMetadata.Count()));
                    Console.WriteLine(string.Format("Start: {0}", DateTime.Now));
                    int cont = 0;
                    foreach (EntityMetadata currentEntity in response.EntityMetadata)
                    {
                        cont++;
                        Console.WriteLine(string.Format("{1} - Updating {0} entity", currentEntity.LogicalName, cont));
                       
                        currentEntity.IsVisibleInMobile.Value = false;

                        UpdateEntityRequest updateEntityRequest = new UpdateEntityRequest();
                        updateEntityRequest.Entity = currentEntity;
                        _serviceProxy.Execute(updateEntityRequest);
                    }
                    Console.WriteLine(string.Format("Publishing..."));
                    PublishAllXmlRequest publishRequest = new PublishAllXmlRequest();
                    _serviceProxy.Execute(publishRequest);
                    Console.WriteLine(string.Format("End: {0}", DateTime.Now));



                }
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>)
            {
                // You can handle an exception here or pass it back to the calling method.
                throw;
            }
        }
    }
}
