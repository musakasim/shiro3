using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using Autofac;
using Autofac.Integration.Mef;
using FrInterfaces;
using Shiro.Controller;
using IContainer = Autofac.IContainer;
using System.ComponentModel.Composition.Hosting;

namespace Shiro
{
    public static class IoCContainer
    {
        public static IContainer BaseContainer { get; private set; }

        //static IoCContainer()
        //{
        //    Build();
        //}

        public static void Build()
        {
            if (BaseContainer == null)
            {
                var builder = new ContainerBuilder();

                // In design mode i don't want to connect real db source
                // In design mode sets the persistency provider to in memory db accessor which only uses objects defined in DesignTimeData class
                //! DbRepository should be single instance since so many cases each instance locks the sources
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                    //though ShiroStaticDataRepository doesn't need singleton, i wanted to conform statement above
                    builder.RegisterType<ShiroStaticDataRepository>().As<IRepository>().SingleInstance();
                else
                {
                    //load db repo from app.config:
                    //builder.RegisterModule(new Autofac.Configuration.ConfigurationSettingsReader());
                    //load db with reference:
                    //builder.RegisterType<StsDbRepository.StsDbRepository>().As<IRepository>() ;
                    //load db by MEF catalog:
                    var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    // ReSharper disable once AssignNullToNotNullAttribute
                    var mefCatalog = new DirectoryCatalog(directoryName);
                    builder.RegisterComposablePartCatalog(mefCatalog);

                    //todo: if no configuration available in app.config or not found with MEF set default to Web based IRepository implementation 
                
                }


                builder.RegisterType<ShiroDictionaryController>().As<IShiroDictionaryController>();
                builder.RegisterType<KanjiInfoController>().As<IKanjiInfoController>();
                builder.RegisterType<TatoebaController>().As<ITatoebaController>();
                //builder.RegisterType<KanjiDic2Controller>().As<IKanjiDic2Controller>();
                //builder.RegisterType<JMDictController>().As<IJMDictController>();
                //builder.RegisterType<KanjiVgController>().As<IKanjiVgController>();
                builder.RegisterType<BookmarkController>().As<IBookmarkController>();
                builder.RegisterType<WritingProgressController>().As<IWritingProgressController>();
                BaseContainer = builder.Build();
            }
        }

        public static TService Resolve<TService>()
        {
            return BaseContainer.Resolve<TService>();
        }
    }
}
