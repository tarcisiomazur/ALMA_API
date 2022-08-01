namespace ALMA_API.Utils;

public static class DotEnv
{
    public static void Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"{filePath} não encontrado. Verifique se o arquivo está no diretório final");
        }

        foreach (var line in File.ReadAllLines(filePath))
        {
            var parts = line.Split(
                '=',
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                continue;

            Environment.SetEnvironmentVariable(parts[0], parts[1]);
        }
    }
}