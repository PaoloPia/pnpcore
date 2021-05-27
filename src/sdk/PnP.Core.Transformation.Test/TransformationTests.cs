﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PnP.Core.Transformation.Services.Builder.Configuration;
using PnP.Core.Transformation.Services.Core;
using PnP.Core.Transformation.Services.MappingProviders;
using PnP.Core.Services;
using PnP.Core.Transformation.Services.Builder;
using PnP.Core.Transformation.SharePoint;
using PnP.Core.Transformation.Test.Utilities;

namespace PnP.Core.Transformation.Test
{
    [TestClass]
    public class TransformationTests
    {

        [TestMethod]
        public async Task SharepointTransformAsync()
        {
            var services = new ServiceCollection();
            services.AddTestPnPCore();
            services.AddPnPSharePointTransformation();

            var provider = services.BuildServiceProvider();

            var targetContext = await provider.GetRequiredService<IPnPContextFactory>().CreateAsync(TestCommon.TargetTestSite);
            // TODO: complete with real uris
            var sourceItem = new SharePointSourceItem(new Uri("http://site/item"));
            var targetUri = new Uri("http://target/item");

            var pageTransformator = provider.GetRequiredService<IPageTransformator>();
            var result = await pageTransformator.TransformAsync(new PageTransformationTask(sourceItem, targetContext, targetUri));

            Assert.IsNotNull(result);
            Assert.Equals(targetUri, result);
        }

        [TestMethod]
        public async Task InMemoryExecutorSharePointTransformAsync()
        {
            var services = new ServiceCollection();
            services.AddTestPnPCore();
            services.AddPnPSharePointTransformation();

            var provider = services.BuildServiceProvider();

            var transformationExecutor = provider.GetRequiredService<ITransformationExecutor>();
            var pnpContextFactory = provider.GetRequiredService<IPnPContextFactory>();
            var result = await transformationExecutor.TransformSharePointAsync(
                pnpContextFactory,
                TestCommon.SourceTestSite,
                TestCommon.TargetTestSite);

            Assert.IsNotNull(result);
            Assert.AreEqual(TransformationExecutionState.Completed, result.State);
            Assert.AreEqual(100, result.Percentage);
        }

    }
}