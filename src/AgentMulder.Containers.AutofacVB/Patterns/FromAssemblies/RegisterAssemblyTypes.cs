using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using AgentMulder.Containers.AutofacVB.Patterns.Helpers;
using AgentMulder.Containers.AutofacVB.Registrations;
using AgentMulder.ReSharper.Domain.Elements.Modules;
using AgentMulder.ReSharper.Domain.Patterns;
using AgentMulder.ReSharper.Domain.Registrations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.VB.Tree;
using JetBrains.ReSharper.Psi.Services.VB.StructuralSearch;
using JetBrains.ReSharper.Psi.Services.VB.StructuralSearch.Placeholders;
using JetBrains.ReSharper.Psi.Services.StructuralSearch;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using IComponentRegistration = AgentMulder.ReSharper.Domain.Registrations.IComponentRegistration;

namespace AgentMulder.Containers.AutofacVB.Patterns.FromAssemblies
{
    [Export("ComponentRegistration", typeof(IRegistrationPattern))]
    public sealed class RegisterAssemblyTypes : FromDescriptorPatternBase
    {
        private static readonly IStructuralSearchPattern pattern =
            new VBStructuralSearchPattern("$builder$.RegisterAssemblyTypes($assemblies$)",
                new ExpressionPlaceholder("builder", "Global.Autofac.ContainerBuilder", true),
                new ArgumentPlaceholder("assemblies", -1, -1));

        [ImportingConstructor]
        public RegisterAssemblyTypes([ImportMany] params IBasedOnPattern[] basedOnPatterns)
            : base(pattern, basedOnPatterns)
        {
            ModuleExtractor.AddExtractor(new AutofacModuleThisAssemblyExtractor());
        }

        public override IEnumerable<IComponentRegistration> GetComponentRegistrations(ITreeNode registrationRootElement)
        {
            IExpressionStatement parentExpression = GetParentExpressionStatemenmt(registrationRootElement);
            if (parentExpression == null)
            {
                yield break;
            }

            IStructuralMatchResult match = Match(registrationRootElement);
            if (match.Matched)
            {
                var arguments = match.GetMatchedElementList("assemblies").Cast<IVBArgument>();

                IEnumerable<IModule> modules = arguments.SelectNotNull(argument => ModuleExtractor.GetTargetModule(argument.Expression));

                foreach (IModule module in modules)
                {
                    var registration = new ModuleBasedOnRegistration(module, new DefaultScanAssemblyRegistration(registrationRootElement));

                    var basedOnRegistrations = BasedOnPatterns.SelectMany(
                        basedOnPattern => basedOnPattern.GetBasedOnRegistrations(parentExpression.Expression));

                    yield return new CompositeRegistration(registrationRootElement, registration, basedOnRegistrations.ToArray());
                }
            }
        }

        private class CompositeRegistration : ComponentRegistrationBase
        {
            private readonly ModuleBasedOnRegistration moduleBasedOnRegistration;
            private readonly IEnumerable<BasedOnRegistrationBase> basedOnRegistrations;

            public CompositeRegistration(ITreeNode registrationElement, ModuleBasedOnRegistration moduleBasedOnRegistration, IEnumerable<BasedOnRegistrationBase> basedOnRegistrations)
                : base(registrationElement)
            {
                this.moduleBasedOnRegistration = moduleBasedOnRegistration;
                this.basedOnRegistrations = basedOnRegistrations;
            }

            public override bool IsSatisfiedBy(ITypeElement typeElement)
            {
                return moduleBasedOnRegistration.IsSatisfiedBy(typeElement) && 
                       basedOnRegistrations.All(registration => registration.IsSatisfiedBy(typeElement));
            }
        }

        private IExpressionStatement GetParentExpressionStatemenmt(ITreeNode node)
        {
            for (var n = node; n != null; n = n.Parent)
            {
                var expressionStatement = n as IExpressionStatement;
                if (expressionStatement != null)
                    return expressionStatement;
            }

            return null;
        }
    }
}