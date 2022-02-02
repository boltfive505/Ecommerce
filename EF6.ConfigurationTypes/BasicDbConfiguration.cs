using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.IO;
using System.Reflection;

namespace EF6.ConfigurationTypes
{
    public class BasicDbConfiguration : DbConfiguration
    {
        public BasicDbConfiguration()
        {
            //add cache method
            string cacheModelLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location), "cache", "model");
            Directory.CreateDirectory(cacheModelLocation);
            var dbModelStore = new DefaultDbModelStore(cacheModelLocation);
            IDbDependencyResolver dependencyResolver = new SingletonDependencyResolver<DbModelStore>(dbModelStore);
            AddDependencyResolver(dependencyResolver);
        }
    }
}
