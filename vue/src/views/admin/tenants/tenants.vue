<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newtenantRule"
        :editRule="tenantRule">
        <template slot="newform" slot-scope="slotProps">
            <FormItem :label="$t('Tenants.TenancyName')" prop="tenancyName">
                <Input v-model="slotProps.createModel.tenancyName" :maxlength="64" :minlength="2"></Input>
            </FormItem>
            <FormItem :label="$t('Public.Name')" prop="name">
                <Input v-model="slotProps.createModel.name" :maxlength="128"></Input>
            </FormItem>
            <!-- <FormItem :label="$t('Tenants.DatabaseConnectionString')+'('+$t('Public.Optional')+')'">
                <Input v-model="slotProps.createModel.connectionString" :maxlength="1024"></Input>
            </FormItem> -->
            <FormItem :label="$t('Tenants.AdminEmailAddress')" prop="adminEmailAddress">
                <Input v-model="slotProps.createModel.adminEmailAddress" type="email" :maxlength="256"></Input>
            </FormItem>
            <FormItem>
                <Checkbox v-model="slotProps.createModel.isActive">{{$t('Public.IsActive')}}</Checkbox>
            </FormItem>
            <p><p>{{$t('Public.DefaultPasswordIs',{pwd:'123qwe'})}}</p></p>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <FormItem :label="$t('Tenants.TenancyName')" prop="tenancyName">
                <Input v-model="slotProps.editModel.tenancyName" :maxlength="64" :minlength="2"></Input>
            </FormItem>
            <FormItem :label="$t('Public.Name')" prop="name">
                <Input v-model="slotProps.editModel.name" :maxlength="128"></Input>
            </FormItem>
            <FormItem>
                <Checkbox v-model="slotProps.editModel.isActive">{{$t('Public.IsActive')}}</Checkbox>
            </FormItem>
            <p><p>{{$t('Public.DefaultPasswordIs',{pwd:'123qwe'})}}</p></p>
        </template>
    </simplePage>
</template>
<script>
import simplePage from "../../../components/simplepage.vue";
import TenantApi from "@/api/tenant";
import Cookies from 'js-cookie';
import appconst from '@/libs/appconst'

export default {
  components: {
    simplePage
  },
  data() {
    return {
      title: "Menu.Pages.Tenants",
      api: TenantApi,
      newtenantRule: {
        tenancyName: [{ required: true }],
        name: [{ required: true }],
        adminEmailAddress: [{ required: true }, { type: "email" }]
      },
      tenantRule: {
        tenancyName: [{ required: true }],
        name: [{ required: true }]
      },
      columnsetting: {
        needAction: false,
        columns: [
          {
            title: this.$t("Tenants.TenancyName"),
            key: "tenancyName"
          },
          {
            title: this.$t("Public.Name"),
            key: "name"
          },
          {
            title: this.$t("Public.IsActive"),
            render: (h, params) => {
              return h("Checkbox", {
                props: {
                  value: params.row.isActive,
                  disabled: true
                }
              });
            }
          },
          {
            title: this.$t('Public.Actions'),
            key: "action",
            width: 200,
            render: (h, params) => {
              return h("div", [
                h(
                  "Button",
                  {
                    props: {
                      type: "primary",
                      size: "small"
                    },
                    style: {
                      marginRight: "5px"
                    },
                    on: {
                      click: () => {
                        this.$refs.simplepage.editModel = params.row;
                        this.$refs.simplepage.showEditModal = true;
                      }
                    }
                  },
                  this.$t('Public.Edit')
                ),
                h(
                  "Button",
                  {
                    props: {
                      type: "info",
                      size: "small"
                    },
                    style: {
                      marginRight: "5px"
                    },
                    on: {
                      click: async () => {
                        this.$Modal.confirm({
                          title: this.$t(''),
                          content: this.$t('Public.Switching') + this.$t(this.title),
                          okText: this.$t('Public.Yes'),
                          cancelText: this.$t('Public.No'),
                          onOk: async () => {
                            let rep = await TenantApi.Switching(params.row.id);
                            var tokenExpireDate = undefined;
                            abp.auth.setToken(rep.data.result.accessToken,tokenExpireDate);
                            abp.utils.setCookieValue(appconst.authorization.encrptedAuthTokenName,rep.data.result.encryptedAccessToken,tokenExpireDate,abp.appPath);
                            Cookies.set('userNameOrEmailAddress', 'admin');
                            Cookies.set('Abp.TenantId',params.row.id);
                            this.$store.commit('clearAllTags');
                            window.location = "/#/home";
                          }
                        });
                      }
                    }
                  },
                  this.$t('Public.Switching')
                ),
                h(
                  "Button",
                  {
                    props: {
                      type: "error",
                      size: "small"
                    },
                    on: {
                      click: async () => {
                        this.$Modal.confirm({
                          title: this.$t(''),
                          content: this.$t('Public.Delete') + this.$t(this.title),
                          okText: this.$t('Public.Yes'),
                          cancelText: this.$t('Public.No'),
                          onOk: async () => {
                            await TenantApi.Delete(params.row.id);
                            await this.$refs.simplepage.getpage();
                          }
                        });
                      }
                    }
                  },
                  this.$t('Public.Delete')
                )
              ]);
            }
          }
        ]
      }
    };
  }
};
</script>