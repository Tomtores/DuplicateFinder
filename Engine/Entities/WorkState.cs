namespace Engine.Entities
{
    public enum WorkState
    {
        Iddle,          //Program is awating input
        ListingFiles,   //Program is indexing files, FilesFound contains number of files found so far.
        Comparing,      //Program is comparing files, FilesFound contains number of files processed and Progress reports the progress. Partial result is available.
        Done,           //Program is finished and returned results.
        Error,           //An error occured, the current operation has been aborted.
        Deleting,        // deleting files
        Marking         // calculating items to display
    }
}