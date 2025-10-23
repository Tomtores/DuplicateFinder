using System.Collections.Generic;

namespace DuplicateFinder.Commands
{
    public class MarkTrashAction
    {
        private List<string> trashList;
        private List<string> keepList;

        public MarkTrashAction(List<string> trashList, List<string> keepList)
        {
            this.trashList = trashList;
            this.keepList = keepList;
        }
    }
}
