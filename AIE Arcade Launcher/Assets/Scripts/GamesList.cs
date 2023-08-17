using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GamesList : MonoBehaviour
{
    public GameObject bannerObject;
    public List<GameObject> banners = new List<GameObject>();

    [Min(1)] public int layers = 3;
    public int desiredBannerCount { get { return layers * 2 - 1; } }

    public float verticalSeparation = 100.0f;
    public float horizontalShrinkage = 20.0f;

    [ContextMenu("Create Banners")]
    void CreateBanners()
    {
        if (layers < 1) layers = 1;

        for (int i = 0; i < banners.Count; i++) {
            DestroyImmediate(banners[i]);
        }
        banners.Clear();

        banners.Add(Instantiate(bannerObject, transform));
        banners[0].name = "Middle";

        for (int i = 1; i < layers; i++) {
            GameObject workingObject = CreateSingleBanner(new Vector2(horizontalShrinkage * i, -verticalSeparation * i));
            workingObject.name = "Down " + i;

            banners.Add(workingObject);


            workingObject = CreateSingleBanner(new Vector2(horizontalShrinkage * i, verticalSeparation * i));
            workingObject.name = "Up " + i;

            banners.Add(workingObject);
        }

        GameObject hiddenBanner = CreateSingleBanner(new Vector2(horizontalShrinkage * (layers - 1), -verticalSeparation * (layers - 1)));
        hiddenBanner.name = "Down Hidden";

        banners.Add(hiddenBanner);


        hiddenBanner = CreateSingleBanner(new Vector2(horizontalShrinkage * (layers - 1), verticalSeparation * (layers - 1)));
        hiddenBanner.name = "Up Hidden";

        banners.Add(hiddenBanner);
    }

    GameObject CreateSingleBanner(Vector2 offset)
    {
        GameObject workingObject = Instantiate(bannerObject, transform);
        RectTransform workingRect = workingObject.GetComponent<RectTransform>();

        workingRect.anchoredPosition += offset;
        return workingObject;
    }

    public void MoveBannersUp()
    {
        if (layers < 2) return;

        List<Vector3> bannerPositions = new List<Vector3>();
        for (int i = 0; i < banners.Count; i++) {
            bannerPositions.Add(banners[i].transform.position);
        }

        MoveTo(banners[0], banners[2].transform.position);

        for (int i = 1; i < layers; i++) {
            int target = i * 2;
            MoveTo(banners[target - 1], banners[Mathf.Clamp(target - 2, 0, banners.Count -  1)].transform.position);
            MoveTo(banners[target], banners[Mathf.Clamp(target + 2, 0, banners.Count - 1)].transform.position);
        }
    }

    public void MoveTo(GameObject movingObject, Vector3 position)
    {
        movingObject.transform.position = position;
    }

    /// <summary>
    /// Gets the GameObject visually representing the list, with 0 being the middle, -1 being visually down, and 1 being visually up.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameObject GetListObject(int index)
    {
        if (index == 0) {
            return banners[0];
        } else if (index > 0) {
            return banners[index * 2];
        } else if (index < 0) {
            return banners[index * -2 - 1];
        }

        // shouldn't be possible to reach here
        return null;
    }
}
