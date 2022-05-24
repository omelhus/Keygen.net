namespace license
{

    public class Document<T>
    {
        public T Data { get; set; }
        public List<Error> Errors { get; set; } = new();
    }
}
