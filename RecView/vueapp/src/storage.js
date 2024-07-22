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
                if (token == null) {
                    console.log(null);
                    commit("setUser", null);
                    return;
                }
                const { data: user } = await axios.get(
                    "https://localhost:7154/api/auth/me",
                    {
                        headers: {
                            Authorization: `Bearer ${localStorage.getItem("token")}`,
                        },
                    }
                );
                console.log(user);
                commit("setUser", user);
            } catch (e) {
                console.log(e);
            }
        },
        async loginSpotify({ commit }) {
            try {
                const response = await axios.get("https://localhost:7154/api/SpotifyAuth/login");
                if (response == null) {
                    console.log(null);
                    commit("setAccessToken", null);
                    return;
                }
                const { data: user } = await axios.get(
                    "https://localhost:7234/api/auth/me",
                    {
                        headers: {
                            Authorization: `Bearer ${localStorage.getItem("token")}`,
                        },
                    }
                );
                console.log(user);
                commit("setUser", user);

            } catch (error) {
                console.error("Ошибка при попытке входа через Spotify:", error);
            }
        },
        async logout({ commit }) {
            commit("setUser", null);
        },
    },
});