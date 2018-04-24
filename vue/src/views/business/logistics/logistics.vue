<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newLogisticRule"
        :editRule="logisticRule">
        <template slot="newform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="$t('Public.Details')" name="detail">
                    <FormItem :label="$t('Logistics.CorporationName')" prop="corporationName">
                        <Input v-model="slotProps.createModel.corporationName" :maxlength="200" :minlength="1"></Input>
                    </FormItem>
                    <FormItem :label="$t('Logistics.CorporationUrl')" prop="corporationUrl">
                        <Input v-model="slotProps.createModel.corporationUrl" :maxlength="50"></Input>
                    </FormItem>
                    <FormItem :label="$t('Logistics.LogisticFlag')" prop="logisticFlag">
                        <Input v-model="slotProps.createModel.logisticFlag" :maxlength="50"></Input>
                    </FormItem>
                </TabPane>
            </Tabs>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="$t('Public.Details')" name="detail">
                    <FormItem :label="$t('Logistics.CorporationName')" prop="corporationName">
                        <Input v-model="slotProps.editModel.corporationName" :maxlength="200" :minlength="1"></Input>
                    </FormItem>
                    <FormItem :label="$t('Logistics.CorporationUrl')" prop="corporationUrl">
                        <Input v-model="slotProps.editModel.corporationUrl" :maxlength="50"></Input>
                    </FormItem>
                    <FormItem :label="$t('Logistics.LogisticFlag')" prop="logisticFlag">
                        <Input v-model="slotProps.editModel.logisticFlag" :maxlength="50" disabled="disabled" ></Input>
                    </FormItem>
                    <FormItem>
                        <Checkbox v-model="slotProps.editModel.isActive">{{$t('Public.IsActive')}}</Checkbox>
                    </FormItem>
                </TabPane>
            </Tabs>
        </template>
    </simplePage>
</template>
<script>
import simplePage from "../../../components/simplepage.vue";
import LogisticApi from "@/api/logistic";

export default {
  components: {
    simplePage
  },
  data() {
    const validateFlag = (rule, value, callback) => {
      if (!value) {
        callback(new Error("logisticFlag is required"));
      } else {
        LogisticApi.Verify(value).then(function(rep) {
          if (rep.data.result) {
            callback();
          } else {
            callback("logisticFlag is exit");
          }
        });
      }
    };
    return {
      title: "Menu.Pages.Logistics",
      api: LogisticApi,
      newLogisticRule: {
        corporationName: [{ required: true }],
        logisticFlag: [{ required: true, validator:validateFlag }]
      },
      logisticRule: {
        corporationName: [{ required: true }],
        logisticFlag: [{ required: true }]
      },
      columnsetting: {
        needAction: true,
        columns: [
          {
            title: this.$t('Logistics.CorporationName'),
            key: "corporationName"
          },
          {
            title: this.$t('Logistics.CorporationUrl'),
            key: "corporationUrl"
          },
          {
            title: this.$t('Logistics.LogisticFlag'),
            key: "logisticFlag"
          },
          {
            title: this.$t('Public.IsActive'),
            render: (h, params) => {
              return h("Checkbox", {
                props: {
                  value: params.row.isActive,
                  disabled: true
                }
              });
            }
          }
        ]
      }
    };
  }
};
</script>