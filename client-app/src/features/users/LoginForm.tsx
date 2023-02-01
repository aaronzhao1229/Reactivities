import { ErrorMessage, Form, Formik } from "formik";
import { observer } from "mobx-react-lite";
import { Button, Header, Label } from "semantic-ui-react";
import MyTextInput from "../../app/common/form/MyTextInput";
import { useStore } from "../../app/stores/store";

export default observer(function LoginForm() {
  const {userStore} = useStore();

  return (
    <Formik 
    initialValues={{email: '', password: '', error: null}} onSubmit={(values, {setErrors}) => userStore.login(values).catch(error => setErrors({error: 'Invalid email or password'}))}>

      {({handleSubmit, isSubmitting, errors}) => (  // we don't need to create a loading indicator when we're using is submitting because Formik is going to recognize the method that we use for onSubmit above and will automatically turn on the submitting and turn it off once it's got a response back fro the method that we call in the onSubmit
          <Form className='ui form' onSubmit={handleSubmit} autoComplete='off'>
            <Header as='h2' content='Login to Reactivities' color="teal" textAlign="center" />
            <MyTextInput placeholder="Email" name='email'/>
            <MyTextInput placeholder="Password" name='password' type='password' />
            <ErrorMessage 
                name="error" render={() => <Label style={{marginBottom: 10}} basic color="red" content={errors.error} />}
            />
            <Button loading={isSubmitting} positive content='Login' type="submit" fluid />
          </Form>
      )}

    </Formik>
  )
})