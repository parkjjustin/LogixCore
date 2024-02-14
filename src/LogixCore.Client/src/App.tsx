import styled from 'styled-components';
import { PersistGate } from "redux-persist/integration/react";
import { Provider } from 'react-redux';
import { Route, Routes } from 'react-router-dom';
import { persistor, store } from './store'
import { LoginModule, LoginScreen, PostTestingButton, TestingButton } from './components';

const AppContainer = styled.div`
    @import url("https://fonts.googleapis.com/css?family=Roboto:400,700&display=swap");

    width: 100%;
    height: 100%;
    font-family: "OpenSans", sans-serif;
    display: flex;
    justify-content: center;
    background-image: linear-gradient(to right, #fc8c0c, #ef5519);
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
                        <Routes>
                            <Route index element={<LoginModule />} />
                            <Route path='test' element={<LoginScreen />} />
                        </Routes>
                    </AppContainer>

                )}
            </PersistGate>
        </Provider>
    )
}

export default App
