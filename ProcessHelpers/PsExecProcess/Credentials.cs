namespace ProcessHelpers
{
    public class Credentials
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public string GetPsToolArgs()
        {
            return string.Format(" -u {0} -p {1}", this.Username, this.Password);
        }
    }
}