using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoffeeSpawner : MonoBehaviour
{
    public GameObject coffeePrefab;
    public float initialSpawnDelay = 1f;
    public float respawnDelay = 5f;
    public List<SpawnArea> spawnAreas = new List<SpawnArea>();

    private GameObject currentCoffee;
    private RectTransform canvasRect;
    private BoxCollider2D coffeeCollider;
    private bool canSpawn = true;

    [System.Serializable]
    public class SpawnArea
    {
        public Transform corner1;
        public Transform corner2;
    }

    void Start()
    {
        GameObject coffeeCanvas = GameObject.Find("CoffeeCanvas");
        if (coffeeCanvas != null)
        {
            canvasRect = coffeeCanvas.GetComponent<RectTransform>();
        }

        if (coffeePrefab != null)
        {
            coffeeCollider = coffeePrefab.GetComponent<BoxCollider2D>();
            if (coffeeCollider == null)
            {
                coffeeCollider = coffeePrefab.AddComponent<BoxCollider2D>();
            }
            coffeeCollider.isTrigger = true;

            if (!coffeePrefab.CompareTag("Coffee"))
            {
                Debug.LogError("Coffee prefab missing 'Coffee' tag! Please tag it in the Inspector.");
            }
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(initialSpawnDelay);

        while (true)
        {
            if (canSpawn && currentCoffee == null)
            {
                TrySpawnCoffee();
            }
            yield return null;
        }
    }

    private void TrySpawnCoffee()
    {
        for (int attempts = 0; attempts < 50; attempts++)
        {
            SpawnArea area = spawnAreas[Random.Range(0, spawnAreas.Count)];
            Vector2 spawnPosition = GetValidPositionInArea(area);

            if (spawnPosition != Vector2.zero)
            {
                currentCoffee = Instantiate(coffeePrefab, canvasRect.transform);
                currentCoffee.tag = "Coffee";

                RectTransform coffeeRect = currentCoffee.GetComponent<RectTransform>();
                if (coffeeRect != null)
                {
                    coffeeRect.anchoredPosition = spawnPosition;
                }

                var collider = currentCoffee.GetComponent<Collider2D>();
                if (collider == null)
                {
                    collider = currentCoffee.AddComponent<BoxCollider2D>();
                }
                collider.isTrigger = true;

                return;
            }
        }
    }

    public void CoffeeCollected()
    {
        if (currentCoffee != null)
        {
            Destroy(currentCoffee);
            currentCoffee = null;
            StartCoroutine(RespawnCooldown());
        }
    }

    private IEnumerator RespawnCooldown()
    {
        canSpawn = false;
        yield return new WaitForSeconds(respawnDelay);
        canSpawn = true;
    }

    Vector2 GetValidPositionInArea(SpawnArea area)
    {
        if (area.corner1 == null || area.corner2 == null) return Vector2.zero;

        Vector2 areaMin = area.corner1.localPosition;
        Vector2 areaMax = area.corner2.localPosition;

        Vector2 realMin = new Vector2(
            Mathf.Min(areaMin.x, areaMax.x),
            Mathf.Min(areaMin.y, areaMax.y));
        Vector2 realMax = new Vector2(
            Mathf.Max(areaMin.x, areaMax.x),
            Mathf.Max(areaMin.y, areaMax.y));

        Vector2 coffeeHalfSize = coffeeCollider.size * 0.5f * coffeePrefab.transform.localScale;
        Vector2 safeMin = realMin + coffeeHalfSize;
        Vector2 safeMax = realMax - coffeeHalfSize;

        if (safeMin.x > safeMax.x || safeMin.y > safeMax.y)
        {
            return Vector2.zero;
        }

        return new Vector2(
            Random.Range(safeMin.x, safeMax.x),
            Random.Range(safeMin.y, safeMax.y));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var area in spawnAreas)
        {
            if (area.corner1 != null && area.corner2 != null)
            {
                Vector3 center = (area.corner1.position + area.corner2.position) / 2;
                Vector3 size = area.corner1.position - area.corner2.position;
                size.x = Mathf.Abs(size.x);
                size.y = Mathf.Abs(size.y);
                size.z = 0.1f;
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}