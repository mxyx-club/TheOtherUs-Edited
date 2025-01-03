using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BepInEx.Unity.IL2CPP.Utils;
using UnityEngine;
using UnityEngine.Networking;
using static TheOtherRoles.CustomCosmetics.CustomHats.CustomHatManager;

namespace TheOtherRoles.CustomCosmetics.CustomHats;

public class HatsLoader : MonoBehaviour
{
    private bool isRunning;
    private bool isSuccessful = true;
    public void FetchHats()
    {
        if (isRunning) return;
        this.StartCoroutine(CoFetchHats());
    }

    private IEnumerator CoFetchHats()
    {
        isRunning = true;
        string localFilePath = Path.Combine(CosmeticsManager.CustomHatsDir, ManifestFileName);

        if (ModOption.localHats)
        {
            LoadLocalHats();
            isRunning = false;
            yield break;
        }

        yield return DownloadHatsConfig(localFilePath);
        isRunning = false;
    }

    private void LoadLocalHats()
    {
        try
        {
            var path = Path.Combine(CosmeticsManager.CustomHatsDir, ManifestFileName);
            Message($"加载本地帽子文件 {path}");
            var localFileContent = File.ReadAllText(path);
            var response = JsonSerializer.Deserialize<HatsConfigFile>(localFileContent, new JsonSerializerOptions
            {
                AllowTrailingCommas = true
            });
            ProcessHatsData(response);
        }
        catch
        {
            Error("不存在本地帽子配置文件.");
        }
    }

    private IEnumerator DownloadHatsConfig(string path)
    {
        var www = new UnityWebRequest
        {
            method = UnityWebRequest.kHttpVerbGET,
            downloadHandler = new DownloadHandlerBuffer()
        };

        Message($"正在下载帽子配置文件: {CosmeticsManager.RepositoryUrl}/{ManifestFileName}");
        www.url = $"{CosmeticsManager.RepositoryUrl}/{ManifestFileName}";

        var operation = www.SendWebRequest();

        while (!operation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.isNetworkError || www.isHttpError)
        {
            Error($"下载帽子配置文件时出错: {www.error}");
            isSuccessful = false;
            yield break;
        }

        try
        {
            if (!Directory.Exists(CosmeticsManager.CustomHatsDir))
            {
                Directory.CreateDirectory(CosmeticsManager.CustomHatsDir);
            }

            File.WriteAllBytes(path, www.downloadHandler.data);
            Message($"帽子清单已保存到: {path}");

            var downloadedFileContent = File.ReadAllText(path);
            var response = JsonSerializer.Deserialize<HatsConfigFile>(downloadedFileContent, new JsonSerializerOptions
            {
                AllowTrailingCommas = true
            });

            ProcessHatsData(response);
        }
        catch (Exception ex)
        {
            isSuccessful = false;
            Error($"未能保存或加载帽子配置文件: {ex.Message}");
        }
        finally
        {
            www.downloadHandler.Dispose();
            www.Dispose();
        }
    }

    private void ProcessHatsData(HatsConfigFile response)
    {
        UnregisteredHats.AddRange(SanitizeHats(response));
        Message($"读取了 {UnregisteredHats.Count} 项帽子");

        if (!isSuccessful || ModOption.localHats)
        {
            Message("在线配置文件无效，取消下载任务。");
            return;
        };

        var toDownload = GenerateDownloadList(UnregisteredHats);

        Message($"准备下载 {toDownload.Count} 项帽子文件");

        this.StartCoroutine(CoDownloadHats(toDownload));
    }

    private static IEnumerator CoDownloadHats(List<string> toDownload)
    {
        foreach (var fileName in toDownload)
        {
            yield return CoDownloadHatAsset(fileName);
        }
    }

    private static IEnumerator CoDownloadHatAsset(string fileName)
    {
        var www = new UnityWebRequest();
        www.SetMethod(UnityWebRequest.UnityWebRequestMethod.Get);
        fileName = fileName.Replace(" ", "%20");
        www.SetUrl($"{CosmeticsManager.RepositoryUrl}/hats/{fileName}");
        www.downloadHandler = new DownloadHandlerBuffer();
        var operation = www.SendWebRequest();

        while (!operation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.isNetworkError || www.isHttpError)
        {
            Error($"{www.error} {fileName}");
            yield break;
        }

        var filePath = Path.Combine(CosmeticsManager.CustomHatsDir, fileName);
        filePath = filePath.Replace("%20", " ");
        var persistTask = File.WriteAllBytesAsync(filePath, www.downloadHandler.data);
        while (!persistTask.IsCompleted)
        {
            Message($"正在下载: {fileName}");
            if (persistTask.Exception != null)
            {
                Error(persistTask.Exception.Message);
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        www.downloadHandler.Dispose();
        www.Dispose();
    }
}