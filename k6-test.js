import http from "k6/http";
import { check } from "k6";

const API_URL = __ENV.API_URL || "http://localhost:5000";
const BASE_URL = __ENV.BASE_URL || "http://localhost:8080";
const SUFFIX = Date.now();

export const options = {
    stages: [
        { duration: "10s", target: 10 }, // ramp up to 10 VUs
        { duration: "10s", target: 50 }, // ramp up to 50 VUs
        { duration: "30s", target: 50 }, // hold at 50 VUs
        { duration: "10s", target: 0 }, // scale back down
    ],
    thresholds: {
        http_req_failed: ["rate<0.01"], // under 1% errors
        http_req_duration: ["p(95)<250", "p(99)<1000"], // 95% < 250ms, 99% < 1s
        checks: ["rate>0.99"], // 99%+ of checks pass
    },
};

export function setup() {
    const userName = `loaduser${SUFFIX}`;
    const email = `${userName}@test.com`;
    const password = "SecurePass123!";

    const registerRes = http.post(
        `${API_URL}/api/v1/account/register`,
        JSON.stringify({ userName, email, password }),
        { headers: { "Content-Type": "application/json" } },
    );

    let token = registerRes.status === 200 ? registerRes.json("token") : null;

    if (!token) {
        const loginRes = http.post(
            `${API_URL}/api/v1/account/login`,
            JSON.stringify({ email, password }),
            { headers: { "Content-Type": "application/json" } },
        );
        if (loginRes.status !== 200) {
            throw new Error(
                `setup login failed: ${loginRes.status} ${loginRes.body}`,
            );
        }
        token = loginRes.json("token");
    }

    const createRes = http.post(
        `${API_URL}/api/v1/shorturl`,
        JSON.stringify({ url: "https://example.com" }),
        {
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
            },
        },
    );

    if (createRes.status !== 201) {
        throw new Error(
            `setup create shorturl failed: ${createRes.status} ${createRes.body}`,
        );
    }

    return { shortCode: createRes.json("shortCode") };
}

export default function (data) {
    const res = http.get(`${BASE_URL}/${data.shortCode}`, { redirects: 0 });
    check(res, { "redirect status is 302": (r) => r.status === 302 });
}
