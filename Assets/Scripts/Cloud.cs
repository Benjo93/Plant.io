using UnityEngine;

public class Cloud : MonoBehaviour
{

    public Sprite cloud; 
    public Vector3 destination { get; set; }
    public ParticleSystem ps;
    private ParticleSystem.ShapeModule shape;
    private ParticleSystem.EmissionModule emission; 

    public float rain { get; set; }
    public float distance { get; set; }

    void Start()
    {
        // Set the cloud sprite.
        GetComponent<SpriteRenderer>().sprite = cloud;

        transform.localScale = transform.localScale * (0.25f + distance);

        // Generate rain system.
        ps = Instantiate(ps, new Vector3(0f, -transform.localScale.y, 0f), Quaternion.identity);

        // Set the rain shape. 
        shape = ps.shape;
        shape.scale = new Vector3(shape.scale.x * transform.localScale.x, 1f, 1f);

        // Initialize the rain rate.
        emission = ps.emission;
        emission.rateOverTime = 0;
        //if (rain > 0.5f) emission.rateOverTime = 10 * rain;
        if (distance > 0.4f)
        {
            emission.rateOverTime = 10 * Enviroment.RAINFALL;
        }

        GetComponent<SpriteRenderer>().color = new Color(1f - rain * 0.5f, 1f - rain * 0.5f, 1f - rain * 0.5f, Random.Range(0.4f, 0.75f));

    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * Enviroment.WIND * (0.5f + distance * 2f));

        shape.position = transform.position;
        
        if (transform.position == destination)
        {
            Destroy(ps);
            Destroy(gameObject);
        }
    }
}
