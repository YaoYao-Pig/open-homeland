using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PictureController : MonoBehaviour
{
    [SerializeField] private RawImage rawImage; // 用于显示相机画面
    [SerializeField] private Button captureButton; // 用于进入拍照模式
    [SerializeField] private Button saveButton; // 用于保存图片
    [SerializeField] private GameObject stickerPanel; // 显示贴纸的 UI 面板
    [SerializeField] private List<DraggableSticker> draggableStickers; // 用于记录所有拖拽过的贴纸
    [SerializeField] private Camera camera; // 用于拍照的相机

    private RenderTexture renderTexture;
    private Texture2D screenshot;

    private void Start()
    {
        captureButton.onClick.AddListener(EnterPhotoMode);
        saveButton.onClick.AddListener(SavePhoto);
        stickerPanel.SetActive(false); // 拍照模式默认不显示贴纸面板
    }

    // 进入拍照模式并立即截图
    void EnterPhotoMode()
    {
        // 创建并分配 RenderTexture，用来作为相机的渲染目标
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        camera.targetTexture = renderTexture;

        // 将渲染结果显示在 RawImage 上
        rawImage.texture = renderTexture;

        // 清空 RenderTexture 内容
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.clear);

        // 截图并保存为 Texture2D
        screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenshot.Apply();

        // 显示贴纸面板
        stickerPanel.SetActive(true);
    }

    // 保存照片
    void SavePhoto()
    {
        if (screenshot == null)
        {
            Debug.LogWarning("没有可保存的截图！");
            return;
        }

        // 创建一个新的 Texture2D 用来合成截图和贴纸
        Texture2D combinedScreenshot = new Texture2D(screenshot.width, screenshot.height, TextureFormat.RGB24, false);

        // 先把截图的内容复制到合成图像中
        combinedScreenshot.SetPixels(screenshot.GetPixels());

        // 合成贴纸到截图中
        foreach (var sticker in draggableStickers)
        {
            // 获取贴纸的纹理和位置
            Texture2D stickerTexture = sticker.GetComponent<Image>().mainTexture as Texture2D;
            if (stickerTexture != null)
            {
                // 获取贴纸的屏幕位置
                Vector2 stickerPosition = sticker.stickerPosition;
                int stickerX = Mathf.FloorToInt(stickerPosition.x);
                int stickerY = Mathf.FloorToInt(stickerPosition.y);

                Color[] stickerPixels = stickerTexture.GetPixels();

                // 将贴纸像素合成到截图中
                for (int y = 0; y < stickerTexture.height; y++)
                {
                    for (int x = 0; x < stickerTexture.width; x++)
                    {
                        // 确保贴纸的像素在截图范围内
                        int targetX = stickerX + x;
                        int targetY = stickerY + y;

                        if (targetX >= 0 && targetX < combinedScreenshot.width && targetY >= 0 && targetY < combinedScreenshot.height)
                        {
                            // 合成贴纸像素
                            Color stickerPixel = stickerPixels[x + y * stickerTexture.width];
                            combinedScreenshot.SetPixel(targetX, targetY, stickerPixel);
                        }
                    }
                }
            }
        }

        // 应用贴纸合成后的图片
        combinedScreenshot.Apply();

        // 保存合成后的图像为 PNG 文件
        byte[] bytes = combinedScreenshot.EncodeToPNG();
        string filePath = Path.Combine(Application.persistentDataPath, "final_screenshot.png");
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("图片已保存：" + filePath);

        // 清理
        camera.targetTexture = null;  // 解除相机的渲染目标
        RenderTexture.active = null;  // 解除 RenderTexture 激活状态
    }
}
