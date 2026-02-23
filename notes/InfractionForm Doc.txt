Infraction Analysis Form

Mechanic where as player talks to suspect, they can pull up and fill out a form that relating to the current case

Each form section is tied to a FormManager.cs script that will track which areas have been marked and with which answer for when the form is submitted. Players are rated based on the accuracy of the form they submitted.

Answers can be either fill in the blank or multiple choice. Fill in the blank can only be completed once the name has been revealed in dialogue, which will set a flag like $[CharName]NameRevealed = true. Then the player can click that blank and fill it out.
Multiple choice answers can be clicked, but can't be erased. These are where the player will make their most important choices, as they can mark certain crimes leniently or harshly depending on their choice. 

Sections (script must :

- [Player Role] Name - FITB (Can be completed at any time)
- Name - FITB
- Primary Violation Classification - MC (Public Disorder, Economic Non-Compliance, Ideological Devaiance, Administrative Obstruction)
- Severity Determination - MC (Minor, Standard, Aggravated)
- Officer Statement Verified - Checkbox

Narrative Note:

Player's role is to help expedite moving people from holding into the appropriate place. Arrests are far more frequent now in this brutalist future, so people need to be moved quickly through the justice system, which often means rapid trials

I want to focus on the bureaucratic process, as the gameplay will consist of the player filling out paperwork while talking to an accused person. I also think the burying of mistakes would make for an interesting incentive for the player to try to enact some good from within the system, with the ultimate message being that benevolent individuals cannot overcome the violence inherent to the system. 

V for Vendetta interests me because of the emphasis on the regime's image by silencing political dissidents and only enforcing the law against those not within the system's inner circle i.e. pedophilic billionaires get away with everything while small-time thieves are harshly and publicly punished.

Needs:
Need to write tutorial script and one additional interview

What does the book need to have in it to work for this game? Here is the page breakdown
1. Intro with logo and spiel about the state
2: Instruction reminder
- Each form comes with a Case ID, the arresting officer's name, and the citizenship tier of the suspect, all filled out for you by our helpful and honorable enforcement officers!
- Your job will be to interview the suspect and fill out some information. For now, just focus on their legal name (very important!), the classification of their violation, and your judgement as to the severity of the infraction. Once you've filled this out, go ahead and stamp the form to submit. Forms take 6-12 hours to process, so you'll know your performance ratings at the end of each shift.
3. Explanation of Violations
- The state defines four violation categories. Each category is comprised of many possible infractions. Don't worry too much about getting the exact category right! There are a lot of suspects to interview, and there are many steps in this process to ensure justice is served!
4. Definitions for the four violation types. Each violation represents a betrayal of one of the state's pillars:
Public Order, Economic Coordination, Ideological Integrity, and Respect for the Civic Process
- 5. I. Public Disorder: Any action that disrupts public stability, communal confidence, or perceived safety.
- Examples: Unauthorized gathering, disruptive speech, property defacement, panic inducement, reckless journalism
- 6. II. Economic Non-Compliance: Failure to adhere to economic directives, labor expectations, or state distribution mandates.
- Examples: Work avoidance, Unauthorized trade, Theft, Hoarding goods.
- 7. III. Ideological Deviance: Expression, endorsement, or tolerance of ideas misaligned with the official doctrine.
- Examples: Distribution of unapproved/banned literature, public criticism of leadership, refusal to recite oath, association with domestic terrorist groups
- 8. IV. Institutional Obstruction: Actions that impede, delay, or complicate official procedural operations or mandated reporting systems.
- Examples: Documentation Irregularity, Filing latency, Verification refusal
9. Definitions of each infraction severity (these can all be on the same page)
- Minor: Level I - Isolated Disruption
-- First occurrence, limited audience exposure, minimal propagation risk
- Standard: Level II - Patterned Deviation
-- Repeat behavior, Measurable influence on others, public visibility
- Aggravated: Level III - Destabilizing Influence
-- Coordinated behavior, intentional amplification, ideological contagion risk, cross-category impact