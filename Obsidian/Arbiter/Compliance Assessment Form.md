In the **Compliance Assessment Form**, in addition to the question answer slots, there is also a 3x3 grid of checkboxes the player can fill out that will influence the interview results and the narrative.  
'I'm Billy Blazer, you stupid pig.'
'Go ahead, check off the little box that says "I had my feelings hurt cuz I'm a lil bitch!"'

### Prefab Architecture
ComplianceAssessmentForm will be a prefab that lives in the scene. When a new subject enters the scene, it inputs the scriptable object associated with that subject and fills the prefab instance with data. This data includes:
- string subjectName (tied to txt_SubjectName object in prefab)
- IDFK 

#### Yarn Variables Needed
In order for this form to work, there have to be Yarn variables established in each subject's Yarn script. When the player wants to fill a piece of the Compliance Assessment Form, the Compliance Form prefab needs to read from the InMemoryVariableStorage thing, which IDK how to do that.
The information needed to fill out the Compliance Form, like $nameRevealed (string) and $occupationRevealed, start every conversation null and get filled by simple yarn commands that fill these variables. There is also a variable representing the truest version of this information, like $subjectName (string), which start each node filled. these exist because sometimes a subject might input a nickname or fake name, and if the player doesn't catch it, they might input that into their form incorrectly. 

Variables:
$subjectName (string)
$nameRevealed (string)
(^^^If these are the same and the player goes to fill out the name slot, it will be correct :D)
$subjectOccupation (string)
$occupationRevealed (string)


#### Infractions

##### Short Titles
Reluctant Answers
Record Inconsistency
Loyalty Evasion
Leadership Skepticism
Institutional Humor
Political Curiosity
Vague Ideology
Administrative Frustration
Unverified Sources
Program Skepticism
Past Nostalgia
Ambiguous Values
Procedural Curiosity
Topic Redirection
Emotional Intensity
Civic Discomfort
Unity Minimization
Suspect Sympathy
Restricted Knowledge
Rehearsed Responses
Confrontational Tone
Terminology Dismissal
Historical Revisionism
Civic Apathy
Ideological Hesitation
Questionable Associations
Personal Autonomy
Patriotic Irony
Value Uncertainty
Institutional Undermining

##### Explanations
Displays reluctance to answer routine questions
Provides answers inconsistent with official records
Avoids direct statements regarding loyalty to the state
Expresses skepticism toward national leadership
Uses humor when discussing state institutions
Demonstrates unusual interest in political matters
Gives vague or noncommittal answers to ideological questions
Shows visible frustration with administrative procedures
References unverified information sources
Questions the necessity of state programs
Speaks nostalgically about previous political conditions
Uses ambiguous language when discussing national values
Exhibits excessive curiosity about interview procedures
Attempts to redirect questioning away from assigned topics
Displays unusual emotional intensity during discussion
Shows discomfort when discussing civic obligations
Minimizes the importance of national unity
Expresses sympathy for individuals under investigation
Demonstrates familiarity with restricted or specialized information
Provides rehearsed or overly cautious responses
Engages in argumentative or confrontational dialogue
Uses dismissive language toward official terminology
References alternative interpretations of national history
Shows lack of enthusiasm for civic participation
Demonstrates hesitation before answering ideological prompts
Mentions associations with politically ambiguous individuals
Expresses belief in personal autonomy over state guidance
Displays ironic tone when discussing patriotism
Indicates uncertainty about fundamental national values
Provides statements that may undermine public confidence in institutions