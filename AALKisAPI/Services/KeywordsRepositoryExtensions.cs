using System.Collections.Generic;

using AALKisAPI.Services;

namespace AALKisAPI.Services
{
    public static class KeywordsRepositoryExtensions
    {
        public static void CreateKeywordList(this IKeywordsRepository repository, IEnumerable<string> keywordNames)
        {
            foreach (var name in keywordNames)
            {
                repository.CreateKeyword(name, noteId: 0);
            }
        }

        public static void DeleteKeywordList(this IKeywordsRepository repository, IEnumerable<string> keywordNames)
        {
            foreach (var name in keywordNames)
            {
                repository.DeleteKeyword(name, noteId: 0);
            }
        }
    }
}
