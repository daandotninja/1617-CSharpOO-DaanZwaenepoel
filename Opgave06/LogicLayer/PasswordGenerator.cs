using System.Collections;
using System.Collections.Generic;

namespace LogicLayer
{
    public class PasswordGenerator : IEnumerable<string>
    {
        private ulong count;
        private ulong maxCount;
        private int passwordLength;

        public int PasswordLength
        {
            get
            {
                return passwordLength;
            }

            private set
            {
                passwordLength = value;
                maxCount = 1;
                for (int i = 0; i < PasswordLength; i++)
                {
                    maxCount *= 26;
                }
            }
        }

        public ulong MaxCount
        {
            get
            {
                return maxCount;
            }
        }

        public ulong Count()
        {
            return count;  
        }

        
        public IEnumerator<string> GetEnumerator()
        {
            PasswordLength = 0;
            count = 0;
            while (true)
            {
                PasswordLength++;
                foreach (var password in GetPasswords(0, new char[PasswordLength]))
                {
                    count++;
                    yield return password;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private IEnumerable<string> GetPasswords(int index, char[] password)
        {
            if (index == PasswordLength)
            {
                yield return new string(password);
            }
            else
            {
                for (char c = 'A'; c <= 'Z'; c++)
                {
                    password[index] = c;
                    foreach (var result in GetPasswords(index + 1, password))
                    {
                        yield return result;

                    }
                }
            }
        }
    }
}
