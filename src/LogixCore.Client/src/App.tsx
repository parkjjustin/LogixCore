import styled from 'styled-components';
import { PersistGate } from "redux-persist/integration/react";
import { Provider } from 'react-redux';
import { Route, Routes } from 'react-router-dom';
import { persistor, store } from './store'
import logo from './assets/logo.png';
import { LoginModule, LoginScreen, PostTestingButton, TestingButton } from './components';

const AppContainer = styled.div`
    @import url("https://fonts.googleapis.com/css?family=Roboto:400,700&display=swap");

    width: 100%;
    height: 100%;
    font-family: "OpenSans", sans-serif;
    display: flex;
    justify-content: center;
    background-image: linear-gradient(to bottom right, #fc8c0c, #ef5519);
    place-items: center;
`

const Nav = styled.nav`
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    padding: 15px;
    width: 100%;
    border-bottom: 1px solid #ccc;
    background: #ffffff;
    box-sizing: border-box;
    text-align: right;
`
const Container = styled.div`
    background: #ffffff;
    width: 70%;
    height: 80%;
    border-radius: 15px;
    box-sizing: border-box;
    box-shadow: 50px 50px 90px -30px #222222;
    position: relative;
`

const ImageLogo = styled.img`
    position: absolute;
    top: 1.5rem;
    left: 3rem;
    width: 125px;
`

const LeftDiv = styled.div`
    width: 35%;
    height: 100%;
    border-radius: 15px 0px 0px 15px;
    background: #ffffff;
    display: inline-block;
    vertical-align: top;
`

const RightDiv = styled.div`
    width: 65%;
    height: 100%;
    background: #eeeeee;
    border-radius: 0px 15px 15px 0px;
    display: inline-block;
`

const App = () => {
    return (
        <Provider store={store}>
            <PersistGate loading={null} persistor={persistor}>
                {() => (
                    <AppContainer>
                        <Nav>
                            <TestingButton />
                            <PostTestingButton />
                        </Nav>
                        <Container>
                            <LeftDiv>
                            <ImageLogo src={logo} alt='Logix' />
                            <Routes>
                                <Route index element={<LoginModule />} />
                                <Route path='test' element={<LoginScreen />} />
                                </Routes>
                            </LeftDiv>
                            <RightDiv>
                            </RightDiv>
                        </Container>
                    </AppContainer>

                )}
            </PersistGate>
        </Provider>
    )
}

export default App
