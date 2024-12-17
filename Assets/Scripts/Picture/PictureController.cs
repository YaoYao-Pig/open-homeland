using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PictureController : MonoBehaviour
{
    [SerializeField] private RawImage rawImage; // ������ʾ�������
    [SerializeField] private Button captureButton; // ���ڽ�������ģʽ
    [SerializeField] private Button saveButton; // ���ڱ���ͼƬ
    [SerializeField] private GameObject stickerPanel; // ��ʾ��ֽ�� UI ���
    [SerializeField] private List<DraggableSticker> draggableStickers; // ���ڼ�¼������ק������ֽ
    [SerializeField] private Camera camera; // �������յ����

    private RenderTexture renderTexture;
    private Texture2D screenshot;

    private void Start()
    {
        captureButton.onClick.AddListener(EnterPhotoMode);
        saveButton.onClick.AddListener(SavePhoto);
        stickerPanel.SetActive(false); // ����ģʽĬ�ϲ���ʾ��ֽ���
    }

    // ��������ģʽ��������ͼ
    void EnterPhotoMode()
    {
        // ���������� RenderTexture��������Ϊ�������ȾĿ��
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        camera.targetTexture = renderTexture;

        // ����Ⱦ�����ʾ�� RawImage ��
        rawImage.texture = renderTexture;

        // ��� RenderTexture ����
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.clear);

        // ��ͼ������Ϊ Texture2D
        screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenshot.Apply();

        // ��ʾ��ֽ���
        stickerPanel.SetActive(true);
    }

    // ������Ƭ
    void SavePhoto()
    {
        if (screenshot == null)
        {
            Debug.LogWarning("û�пɱ���Ľ�ͼ��");
            return;
        }

        // ����һ���µ� Texture2D �����ϳɽ�ͼ����ֽ
        Texture2D combinedScreenshot = new Texture2D(screenshot.width, screenshot.height, TextureFormat.RGB24, false);

        // �Ȱѽ�ͼ�����ݸ��Ƶ��ϳ�ͼ����
        combinedScreenshot.SetPixels(screenshot.GetPixels());

        // �ϳ���ֽ����ͼ��
        foreach (var sticker in draggableStickers)
        {
            // ��ȡ��ֽ�������λ��
            Texture2D stickerTexture = sticker.GetComponent<Image>().mainTexture as Texture2D;
            if (stickerTexture != null)
            {
                // ��ȡ��ֽ����Ļλ��
                Vector2 stickerPosition = sticker.stickerPosition;
                int stickerX = Mathf.FloorToInt(stickerPosition.x);
                int stickerY = Mathf.FloorToInt(stickerPosition.y);

                Color[] stickerPixels = stickerTexture.GetPixels();

                // ����ֽ���غϳɵ���ͼ��
                for (int y = 0; y < stickerTexture.height; y++)
                {
                    for (int x = 0; x < stickerTexture.width; x++)
                    {
                        // ȷ����ֽ�������ڽ�ͼ��Χ��
                        int targetX = stickerX + x;
                        int targetY = stickerY + y;

                        if (targetX >= 0 && targetX < combinedScreenshot.width && targetY >= 0 && targetY < combinedScreenshot.height)
                        {
                            // �ϳ���ֽ����
                            Color stickerPixel = stickerPixels[x + y * stickerTexture.width];
                            combinedScreenshot.SetPixel(targetX, targetY, stickerPixel);
                        }
                    }
                }
            }
        }

        // Ӧ����ֽ�ϳɺ��ͼƬ
        combinedScreenshot.Apply();

        // ����ϳɺ��ͼ��Ϊ PNG �ļ�
        byte[] bytes = combinedScreenshot.EncodeToPNG();
        string filePath = Path.Combine(Application.persistentDataPath, "final_screenshot.png");
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("ͼƬ�ѱ��棺" + filePath);

        // ����
        camera.targetTexture = null;  // ����������ȾĿ��
        RenderTexture.active = null;  // ��� RenderTexture ����״̬
    }
}
