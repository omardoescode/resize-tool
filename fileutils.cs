namespace FileHandling
{
    public static class FileUtils{
        public static bool checkFileExists(string path) {
            return File.Exists(path);
        }
    }
}