namespace School_BookingPortal.Services.FilesServices
{
    public interface IFileService
    {
        public Tuple<int, string> SaveImage(IFormFile imageFile);
        public Tuple<int, string> Savefile(IFormFile certificateFile);
    }
}
