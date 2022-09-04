using Salaros.Configuration;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace RegexDedupe
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Need 3 arguments: Reference file, target file, regex config .ini file");
            } else
            {

                string referenceFile = args[0];
                string targetFile = args[1];
                string configFile = args[2];

                ConfigParser cp = new ConfigParser(configFile);
                string referenceRegexString = cp.GetValue("regex", "referenceRegex");
                string targetRegexString = cp.GetValue("regex", "targetRegex");
                string[] matchKeys = cp.GetValue("regex", "matchKeys").Split(",");
                Regex referenceRegex = new Regex(referenceRegexString.Trim(), RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
                Regex targetRegex = new Regex(targetRegexString.Trim(), RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

                string referenceData = File.ReadAllText(referenceFile);
                string targetData = File.ReadAllText(targetFile);

                MatchCollection referenceMatches = referenceRegex.Matches(referenceData);
                int dupesFound = 0;
                string filteredTargetData = targetRegex.Replace(targetData, (Match match) => {
                    foreach(Match referenceMatch in referenceMatches)
                    {
                        bool misMatch = false;
                        foreach(string matchKey in matchKeys)
                        {
                            if (match.Groups[matchKey].Value != referenceMatch.Groups[matchKey].Value)
                            {
                                misMatch = true;
                                break;
                            }
                        }
                        if (!misMatch)
                        {
                            // This is in fact a dupe. Get rid of it.
                            dupesFound++;
                            return "";
                        }
                    }

                    // No dupe detected. Return normal value.
                    return match.Value;
                });

                if(dupesFound > 0)
                {
                    Console.WriteLine($"{dupesFound} dupes found and removed.");
                    string outputFileName = GetUnusedFilename(targetFile + ".deduped" + Path.GetExtension(targetFile));
                    File.WriteAllText(outputFileName, filteredTargetData);
                } else
                {
                    Console.WriteLine("No dupes found.");
                }

            }
#if DEBUG
            Console.ReadKey();
#endif
        }


        public static string GetUnusedFilename(string baseFilename)
        {
            if (!File.Exists(baseFilename))
            {
                return baseFilename;
            }
            string extension = Path.GetExtension(baseFilename);

            int index = 1;
            while (File.Exists(Path.ChangeExtension(baseFilename, "." + (++index) + extension))) ;

            return Path.ChangeExtension(baseFilename, "." + (index) + extension);
        }
    }
}
