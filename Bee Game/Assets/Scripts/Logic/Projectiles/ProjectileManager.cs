using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    private List<Projectile> projectileList = new List<Projectile>();
    public static ProjectileManager instance;
    [SerializeField]
    private Projectile projectileTemplate;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        projectileTemplate.gameObject.SetActive(false);
    }

    private Projectile CreateProjectile()
    {
        Projectile projectile = Instantiate(projectileTemplate) as Projectile;
        projectile.transform.parent = projectileTemplate.transform.parent;
        projectileList.Add(projectile);

        return projectile;
    }

    private Projectile AvailableProjectile()
    {
        foreach (Projectile projectile in projectileList)
        {
            if (!projectile.IsActive())
                return projectile;
        }

        return CreateProjectile();
    }

    public void ShootProjectile(SelectableLogic target, CreatureLogic source)
    {
        Projectile projectile = AvailableProjectile();
        projectile.gameObject.SetActive(true);
        projectile.SetParameters(target, source);
    }
}
