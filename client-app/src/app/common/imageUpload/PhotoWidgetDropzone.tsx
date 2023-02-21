import React, {useCallback} from 'react'
import {useDropzone} from 'react-dropzone'
import { Header, Icon } from 'semantic-ui-react'

interface Props {
  setFiles: (files: any) => void  // there is an object of File we could use for this file type, but what we also want to do is add an additional property so that we can add a preview to the file itself. and then we can display that in our second part of our component. So rather than extending the file interface and adding that property, what we'll do is just cheat and use any
}

export default function PhotoWidgetDropzone({setFiles}: Props) {
  // create a style for when the dropzone is inactive and we'll create an additional style for when it's active
  const dzStyles = {
    border: 'dashed 3px #eee',
    borderColor: '#eee',
    borderRadius: '5px',
    paddingTop: '30px',
    textAlign: 'center' as 'center', // to get rid of the TypeScript error
    height: 200
  }

  const dzActive = {
    borderColor: 'green'
  }

  // useCallback function returns a memorized version of the callback that only changes if one of the dependencies has changed. So it's really just an optimization feature. any of the setFiles function changes that it would recreate this function in order to use a new one. So it's just going to reuse the setfiles function by using the memorized version of whatever's inside the useCallback

  const onDrop = useCallback((acceptedFiles: any) => {
    setFiles(acceptedFiles.map((file: any) => Object.assign(file, {
      preview: URL.createObjectURL(file)
    }) ))
  }, [setFiles])

  const {getRootProps, getInputProps, isDragActive} = useDropzone({onDrop})

  return (
    <div {...getRootProps()} style={isDragActive ? {...dzStyles, ...dzActive} : dzStyles}>
      <input {...getInputProps()} />
      <Icon name='upload' size='huge' />
      <Header content='Drop image here' />
    </div>
  )
}