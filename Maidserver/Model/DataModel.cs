namespace MaidServer
{
    /// <summary>
    /// 用户基类
    /// </summary>
    internal class Users
    {
        public int account { get; set; }
        public string name { get; set; }
        private string passwd { get; set; }
        public int identity { get; set; }
        public Users(int userAccount, string userName, string userPasswd, int userIdentity){
            this.account = userAccount;
            this.name = userName;
            this.passwd = userPasswd;
            this.identity = userIdentity;
        }
        /// <summary>
        /// 用于验证密码是否正确
        /// </summary>
        /// <param name="passwd">用户的密码</param>
        /// <returns>Type(bool) true or false</returns>
        public bool checkPasswd(string passwd)
        {
            if (algorithm.kmp(passwd, this.passwd))
                return true;
            return false;
        }

        
    }
    /// <summary>
    /// Some algorithm used in this project
    /// </summary>
    internal abstract class algorithm
    {
        /// <summary>
        /// 用于计算KMP单模匹配的前缀数组
        /// </summary>
        /// <param name="T">模式串</param>
        /// <param name="next">前缀数组的引用</param>
        private static void GetNext(string T, int[] next)
        {
            int i = 0, j = 0;
            next[0] = 0;
            while ((i + 1) < T.Length)
            {
                ++i;
                ++j;
                if(T[i] == T[j - 1])
                {
                    next[i] = j;
                }
                else 
                {
                    next[i] = next[j - 1];
                }
            }
        }
        /// <summary>
        /// KMP单模匹配算法，用于检查两个字符串是否相等
        /// </summary>
        /// <param name="S">字符串1</param>
        /// <param name="T">字符串2</param>
        /// <returns>Type(bool), true or false</returns>
        public static bool kmp(string S, string T)
        {
            if(S.Length != T.Length)
                return false;
            int i = 0, j = 0;
            int [] next = new int[T.Length];
            GetNext(T, next);
            while (i < S.Length && j < T.Length)
            {
                if (j == 0 || S[i] == T[j])
                {
                    ++i;
                    ++j;
                }
                else
                    j = next[j - 1];
                
            }
            if (j >= T.Length)
            {
                if(i - T.Length == 0) 
                    return true;
                else 
                    return false;
            }
            else
                return false;

        }

        
    }

}