import { observer } from "mobx-react-lite";
import React from "react";
import { Button, Divider, Grid, Header, Item, Reveal, Segment, Statistic } from "semantic-ui-react";
import { Profile } from "../../app/models/profile";

interface Props {
  profile: Profile;
}
// because this profile object is coming from our store, even though we're not accessing the store directly inside this component, we still need to make it an observer, otherwise we won't react to any changes in our observable inside this component

export default observer(function ProfileHeader({profile} : Props) {
  return (
    <Segment>
            <Grid>
                <Grid.Column width={12}>
                    <Item.Group>
                        <Item>
                            <Item.Image
                                avatar
                                size='small'
                                src={ profile.image || '/assets/user.png'}
                            />
                            <Item.Content verticalAlign='middle'>
                                <Header as='h1' content={profile.displayName}/>
                            </Item.Content>
                        </Item>
                    </Item.Group>
                </Grid.Column>
                <Grid.Column width={4}>
                    <Statistic.Group widths={2}>
                        <Statistic label='Followers' value='5' />
                        <Statistic label='Following' value='42' />
                    </Statistic.Group>
                    <Divider />
                    <Reveal animated='move'>
                        <Reveal.Content visible style={{ width: '100%' }}>
                            <Button
                                fluid
                                color='teal'
                                content='Following'
                            />
                        </Reveal.Content>
                        <Reveal.Content hidden>
                            <Button
                                fluid
                                basic
                                color={true ? 'red' : 'green'}
                                content={true ? 'Unfollow' : 'Follow'}
                            />
                        </Reveal.Content>
                    </Reveal>
                </Grid.Column>
            </Grid>
        </Segment>

  )
})