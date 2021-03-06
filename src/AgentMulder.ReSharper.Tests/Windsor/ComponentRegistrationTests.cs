﻿using System;
using AgentMulder.Containers.CastleWindsor;
using AgentMulder.Containers.CastleWindsor.Providers;
using AgentMulder.ReSharper.Domain.Containers;
using AgentMulder.ReSharper.Tests.Windsor.Helpers;

namespace AgentMulder.ReSharper.Tests.Windsor
{
    [TestWindsor]
    public class ComponentRegistrationTests : AgentMulderTestBase
    {
        protected override string RelativeTestDataPath
        {
            get { return @"Windsor\ComponentTestCases"; }
        }

        protected override IContainerInfo ContainerInfo
        {
            get
            {
                return new WindsorContainerInfo(new[]
                {
                    new ComponentRegistrationProvider(new ImplementedByRegistrationProvider())
                });
            }
        }
    }
}