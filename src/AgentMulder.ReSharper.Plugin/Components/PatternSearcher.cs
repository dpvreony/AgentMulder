using System.Collections.Generic;
using AgentMulder.ReSharper.Domain.Patterns;
using JetBrains.Application.Progress;
using JetBrains.DocumentManagers;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Features.StructuralSearch.Finding;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Services.StructuralSearch;
using JetBrains.ReSharper.Psi.Services.StructuralSearch.Impl;

namespace AgentMulder.ReSharper.Plugin.Components
{
    [SolutionComponent]
    public class PatternSearcher
    {
        private readonly DocumentManager documentManager;

        public PatternSearcher(DocumentManager documentManager)
        {
            this.documentManager = documentManager;
        }

        public IEnumerable<IStructuralMatchResult> Search(IStructuralPatternHolder pattern, ISearchDomain searchDomain)
        {
            var results = new List<IStructuralMatchResult>();
            var consumer = new FindResultConsumer<IStructuralMatchResult>(result =>
            {
                var findResultStructural = result as FindResultStructural;
                if (findResultStructural != null && findResultStructural.DocumentRange.IsValid())
                {
                    return findResultStructural.MatchResult;
                }

                return null;
            }, match =>
            {
                if (match != null)
                {
                    results.Add(match);
                }
                return FindExecution.Continue;
            });

            DoSearch(pattern.Matcher, consumer, searchDomain);

            return results;
        }

        private void DoSearch(IStructuralMatcher matcher, IFindResultConsumer<IStructuralMatchResult> consumer, ISearchDomain searchDomain)
        {
            // todo add support for VB (eventually)
            var searcher = new StructuralSearcher(documentManager, CSharpLanguage.Instance, matcher);
            var searchDomainSearcher = new StructuralSearchDomainSearcher<IStructuralMatchResult>(
                searchDomain, searcher, consumer, NullProgressIndicator.Instance, true);

            searchDomainSearcher.Run();
        }
    }
}