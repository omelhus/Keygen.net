namespace license
{

    public class DocumentWithMeta<T, U> : Document<T>
    {
        public U Meta { get; set; }
    }
}
