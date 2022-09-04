# RegexDedupe

Need to get rid of duplicates between two text files, but both text files have different formatting, making simple duplicate line removal impossible? Why not use regular expressions to identify records in both files?

For example, you have a text file with filenames and a CSV file with file records. You want to remove any file records from the CSV file that match any of the filenames in the text file.

This tool was made to help you.

#### Usage

```sh
RegexDedupe referenceFile.txt targetFile.csv regexConfig.ini
```

And inside the regexConfig.ini, we define the search patterns:

```ini
[regex]
referenceRegex=^(?<filename>.*?)$
targetRegex=^".*?","(?<filename>.*?)",".*?".*?$
matchKeys=filename
```

The ```referenceRegex``` key in the ini file finds records in the reference file. The ```targetRegex``` key finds records in the target file. And the ```matchKeys``` key defines the named regex groups which are used for determining if the two records are identical. You can use multiple regex groups for matching by separating them with a comma like:

```ini
matchKeys=group1,group2,group3
```

*Do not leave any spaces around the commas.*

So for example, the above example config would work with the following reference and target files:

referenceFile.txt:
```
file1.txt
file3.txt
file5.txt
```

targetFile.csv:
```csv
"Testfile #2","file2.txt","350KB"
"Testfile #3","file3.txt","450KB"
"Testfile #4","file4.txt","650KB"
```

Given the example config above and these two files, the middle entry in targetFile.csv will be replaced by empty space. 