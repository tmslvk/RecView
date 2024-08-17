import { createStore } from "vuex";
import axios from "axios";
import router from "./router";

export const store = createStore({
    state: {
        user: null,
        accessToken: null,
    },
    mutations: {
        setUser(state, user) {
            state.user = user;
        },
        setAccessToken(state, token) {
            state.accessToken = token;
        }
    },
    actions: {
        async setUser({ commit }) {
            try {
                const token = localStorage.getItem("token");
                if (!token) {
                    commit("setUser", null);
                    return;
                }

                const { data: user } = await axios.get(
                    "https://localhost:7154/api/Auth/me",
                    {
                        headers: {
                            Authorization: `Bearer ${token}`,
                        },
                    }
                );
                commit("setUser", user);
            } catch (e) {
                console.error(e);
                commit("setUser", null);
            }
        },
        async handleSpotifyCallback({ dispatch }) {
            try {
                const urlParams = new URLSearchParams(window.location.search);
                const code = urlParams.get("code");

                if (!code) {
                    throw new Error("Authorization code not found.");
                }

                const { data } = await axios.get(`https://localhost:7154/api/SpotifyAuth/callback?code=${code}`);
                const { Token } = data;

                localStorage.setItem("token", Token);
                await dispatch('setUser');

                router.push({ path: '/RegistrationSpotify' });
            } catch (error) {
                console.error("Error occurred during Spotify callback handling:", error);
            }
        },
        async logout({ commit }) {
            // Clear token from storage
            localStorage.removeItem("token");
            commit("setUser", null);
            // Optionally redirect to login or home page
            router.push({ path: '/Login' });
        },
    },
});
